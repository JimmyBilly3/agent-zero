from fastapi import APIRouter, HTTPException
from fastapi.responses import StreamingResponse
from pydantic import BaseModel
from typing import Optional
import io
import logging

from models.text_to_speech import TextToSpeech

logger = logging.getLogger(__name__)
router = APIRouter()

# Initialize TTS engine
tts_engine = TextToSpeech()

class TTSRequest(BaseModel):
    text: str
    language: str
    voice: Optional[str] = "default"

@router.post("/tts")
async def text_to_speech(request: TTSRequest):
    """
    Convert text to speech audio.

    Supports:
    - Languages: en, zh, th
    - Voices: male, female, default
    """
    try:
        logger.info(f"TTS request for language: {request.language}, voice: {request.voice}")

        # Validate language
        supported_langs = ['en', 'zh', 'th']
        if request.language not in supported_langs:
            raise HTTPException(
                status_code=400,
                detail=f"Unsupported language. Supported: {', '.join(supported_langs)}"
            )

        # Generate audio
        audio_data = await tts_engine.synthesize(
            text=request.text,
            language=request.language,
            voice=request.voice
        )

        # Return audio as streaming response
        return StreamingResponse(
            io.BytesIO(audio_data),
            media_type="audio/mpeg",
            headers={
                "Content-Disposition": "attachment; filename=speech.mp3"
            }
        )

    except Exception as e:
        logger.error(f"TTS error: {str(e)}")
        raise HTTPException(status_code=500, detail=f"Text-to-speech failed: {str(e)}")
