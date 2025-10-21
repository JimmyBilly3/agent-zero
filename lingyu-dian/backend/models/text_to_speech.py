import os
import logging
from typing import Optional
import asyncio
import tempfile

logger = logging.getLogger(__name__)

class TextToSpeech:
    """
    Handles text-to-speech conversion using edge-tts.
    """

    def __init__(self):
        self.voices = {
            'en': {
                'male': 'en-US-GuyNeural',
                'female': 'en-US-JennyNeural',
                'default': 'en-US-AriaNeural'
            },
            'zh': {
                'male': 'zh-CN-YunxiNeural',
                'female': 'zh-CN-XiaoxiaoNeural',
                'default': 'zh-CN-XiaoxiaoNeural'
            },
            'th': {
                'male': 'th-TH-NiwatNeural',
                'female': 'th-TH-PremwadeeNeural',
                'default': 'th-TH-PremwadeeNeural'
            }
        }

    async def synthesize(
        self,
        text: str,
        language: str = "en",
        voice: str = "default"
    ) -> bytes:
        """
        Convert text to speech audio.

        Args:
            text: Text to convert
            language: Language code (en, zh, th)
            voice: Voice type (male, female, default)

        Returns:
            Audio data as bytes
        """
        try:
            import edge_tts

            # Get voice name
            voice_name = self.voices.get(language, {}).get(voice, self.voices['en']['default'])

            # Generate speech
            communicate = edge_tts.Communicate(text, voice_name)

            # Save to temporary file
            with tempfile.NamedTemporaryFile(delete=False, suffix=".mp3") as temp_file:
                temp_path = temp_file.name

            await communicate.save(temp_path)

            # Read audio data
            with open(temp_path, 'rb') as f:
                audio_data = f.read()

            # Clean up
            os.remove(temp_path)

            logger.info(f"TTS successful for language: {language}, voice: {voice}")
            return audio_data

        except Exception as e:
            logger.error(f"TTS error: {e}")
            # Return silent audio as fallback
            return self._generate_silent_audio()

    def _generate_silent_audio(self) -> bytes:
        """Generate a short silent audio file as fallback."""
        # Simple WAV header for 1 second of silence
        # This is a minimal WAV file that most players can handle
        return b'RIFF$\x00\x00\x00WAVEfmt \x10\x00\x00\x00\x01\x00\x01\x00"V\x00\x00D\xac\x00\x00\x02\x00\x10\x00data\x00\x00\x00\x00'
