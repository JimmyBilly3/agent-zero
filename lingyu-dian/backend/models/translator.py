import os
import logging
from typing import Dict, Any, Optional
from openai import OpenAI
from anthropic import Anthropic

logger = logging.getLogger(__name__)

class Translator:
    """
    Handles text translation with online and offline modes.

    Online: Uses OpenAI GPT-4 or Anthropic Claude
    Offline: Uses MarianMT models (fallback)
    """

    def __init__(self):
        self.openai_client = None
        self.anthropic_client = None
        self.offline_models = {}

        # Initialize online clients
        openai_key = os.getenv("OPENAI_API_KEY")
        anthropic_key = os.getenv("ANTHROPIC_API_KEY")

        if openai_key:
            try:
                self.openai_client = OpenAI(api_key=openai_key)
                logger.info("OpenAI client initialized")
            except Exception as e:
                logger.warning(f"Failed to initialize OpenAI: {e}")

        if anthropic_key:
            try:
                self.anthropic_client = Anthropic(api_key=anthropic_key)
                logger.info("Anthropic client initialized")
            except Exception as e:
                logger.warning(f"Failed to initialize Anthropic: {e}")

        # Initialize offline models (lazy loading)
        self.offline_enabled = os.getenv("ENABLE_OFFLINE_MODE", "True").lower() == "true"

    async def translate(
        self,
        text: str,
        source_lang: str,
        target_lang: str,
        tone: str = "casual",
        offline: bool = False
    ) -> Dict[str, Any]:
        """
        Translate text from source to target language.

        Args:
            text: Text to translate
            source_lang: Source language code (en, zh, th)
            target_lang: Target language code (en, zh, th)
            tone: Translation tone (formal, casual, classical)
            offline: Use offline mode

        Returns:
            Dict with translated_text, confidence, and method
        """
        if offline or not (self.openai_client or self.anthropic_client):
            return await self._translate_offline(text, source_lang, target_lang, tone)
        else:
            return await self._translate_online(text, source_lang, target_lang, tone)

    async def _translate_online(
        self,
        text: str,
        source_lang: str,
        target_lang: str,
        tone: str
    ) -> Dict[str, Any]:
        """Translate using online API (OpenAI or Anthropic)."""
        try:
            # Language names for better prompting
            lang_names = {
                'en': 'English',
                'zh': 'Chinese (Simplified)',
                'th': 'Thai'
            }

            # Tone instructions
            tone_instructions = {
                'formal': 'Use formal and professional language.',
                'casual': 'Use casual and conversational language.',
                'classical': 'Use classical and literary style (文言文 for Chinese).'
            }

            # Build prompt
            prompt = f"""Translate the following text from {lang_names[source_lang]} to {lang_names[target_lang]}.

{tone_instructions.get(tone, '')}

Text to translate:
{text}

Provide only the translation, without any explanations or additional text."""

            # Try OpenAI first
            if self.openai_client:
                try:
                    response = self.openai_client.chat.completions.create(
                        model="gpt-4",
                        messages=[
                            {"role": "system", "content": "You are a professional translator specializing in English, Chinese, and Thai languages."},
                            {"role": "user", "content": prompt}
                        ],
                        temperature=0.3,
                        max_tokens=2000
                    )
                    translated_text = response.choices[0].message.content.strip()
                    logger.info(f"Translation successful via OpenAI")

                    return {
                        'translated_text': translated_text,
                        'confidence': 0.95,
                        'method': 'online-openai'
                    }
                except Exception as e:
                    logger.warning(f"OpenAI translation failed: {e}")

            # Try Anthropic as fallback
            if self.anthropic_client:
                try:
                    response = self.anthropic_client.messages.create(
                        model="claude-3-sonnet-20240229",
                        max_tokens=2000,
                        messages=[
                            {"role": "user", "content": prompt}
                        ]
                    )
                    translated_text = response.content[0].text.strip()
                    logger.info(f"Translation successful via Anthropic")

                    return {
                        'translated_text': translated_text,
                        'confidence': 0.95,
                        'method': 'online-anthropic'
                    }
                except Exception as e:
                    logger.warning(f"Anthropic translation failed: {e}")

            # If all online methods fail, fallback to offline
            logger.warning("All online translation methods failed, falling back to offline")
            return await self._translate_offline(text, source_lang, target_lang, tone)

        except Exception as e:
            logger.error(f"Online translation error: {e}")
            return await self._translate_offline(text, source_lang, target_lang, tone)

    async def _translate_offline(
        self,
        text: str,
        source_lang: str,
        target_lang: str,
        tone: str
    ) -> Dict[str, Any]:
        """Translate using offline models (MarianMT or simple fallback)."""
        try:
            # Try to use MarianMT if available
            if self.offline_enabled:
                try:
                    from transformers import MarianMTModel, MarianTokenizer

                    model_name = self._get_marian_model_name(source_lang, target_lang)
                    if model_name:
                        # Load model (cached after first load)
                        model_key = f"{source_lang}_{target_lang}"
                        if model_key not in self.offline_models:
                            tokenizer = MarianTokenizer.from_pretrained(model_name)
                            model = MarianMTModel.from_pretrained(model_name)
                            self.offline_models[model_key] = (tokenizer, model)
                            logger.info(f"Loaded offline model: {model_name}")

                        tokenizer, model = self.offline_models[model_key]

                        # Translate
                        inputs = tokenizer(text, return_tensors="pt", padding=True, truncation=True, max_length=512)
                        outputs = model.generate(**inputs)
                        translated_text = tokenizer.decode(outputs[0], skip_special_tokens=True)

                        logger.info(f"Translation successful via offline model")
                        return {
                            'translated_text': translated_text,
                            'confidence': 0.75,
                            'method': 'offline-marianmt'
                        }
                except Exception as e:
                    logger.warning(f"Offline model translation failed: {e}")

            # Simple fallback (for demo purposes)
            logger.info("Using simple fallback translation")
            return {
                'translated_text': f"[Offline mode - translation would appear here]\n{text}",
                'confidence': 0.5,
                'method': 'offline-fallback'
            }

        except Exception as e:
            logger.error(f"Offline translation error: {e}")
            raise Exception(f"Translation failed: {str(e)}")

    def _get_marian_model_name(self, source_lang: str, target_lang: str) -> Optional[str]:
        """Get MarianMT model name for language pair."""
        models = {
            ('en', 'zh'): 'Helsinki-NLP/opus-mt-en-zh',
            ('zh', 'en'): 'Helsinki-NLP/opus-mt-zh-en',
            ('en', 'th'): 'Helsinki-NLP/opus-mt-en-th',
            ('th', 'en'): 'Helsinki-NLP/opus-mt-th-en',
        }
        return models.get((source_lang, target_lang))
