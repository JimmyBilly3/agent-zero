import os
import logging
from typing import Dict, Any
import tempfile
import subprocess

logger = logging.getLogger(__name__)

class SpeechRecognizer:
    """
    Handles speech-to-text conversion using Whisper.
    """

    def __init__(self):
        self.model = None
        self.model_loaded = False

    async def transcribe(self, audio_path: str, language: str = "en") -> Dict[str, Any]:
        """
        Transcribe audio to text.

        Args:
            audio_path: Path to audio file
            language: Language code (en, zh, th)

        Returns:
            Dict with text and confidence
        """
        try:
            # Try using openai-whisper library
            if not self.model_loaded:
                try:
                    import whisper
                    self.model = whisper.load_model("base")
                    self.model_loaded = True
                    logger.info("Whisper model loaded")
                except Exception as e:
                    logger.warning(f"Failed to load Whisper model: {e}")

            if self.model:
                # Transcribe with Whisper
                result = self.model.transcribe(
                    audio_path,
                    language=self._get_whisper_lang(language)
                )

                return {
                    'text': result['text'].strip(),
                    'confidence': 0.9
                }

            # Fallback: Simple mock response
            logger.warning("Using fallback speech recognition")
            return {
                'text': "[Speech recognition would appear here - Whisper not available]",
                'confidence': 0.5
            }

        except Exception as e:
            logger.error(f"Speech recognition error: {e}")
            raise Exception(f"Speech recognition failed: {str(e)}")

    def _get_whisper_lang(self, lang_code: str) -> str:
        """Convert language code to Whisper language code."""
        mapping = {
            'en': 'en',
            'zh': 'zh',
            'th': 'th'
        }
        return mapping.get(lang_code, 'en')
