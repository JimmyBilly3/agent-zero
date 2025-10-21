from fastapi import APIRouter, UploadFile, File, Form, HTTPException
from pydantic import BaseModel
import os
import tempfile
import logging

from models.speech_recognizer import SpeechRecognizer

logger = logging.getLogger(__name__)
router = APIRouter()

# Initialize speech recognizer
speech_recognizer = SpeechRecognizer()

class SpeechResponse(BaseModel):
    text: str
    language: str
    confidence: Optional[float] = None

@router.post("/speech", response_model=SpeechResponse)
async def speech_to_text(
    audio: UploadFile = File(...),
    language: str = Form("en")
):
    """
    Convert speech audio to text using Whisper.

    Supports:
    - Languages: en, zh, th
    - Audio formats: wav, mp3, webm, ogg
    """
    try:
        logger.info(f"Speech-to-text request for language: {language}")

        # Validate language
        supported_langs = ['en', 'zh', 'th']
        if language not in supported_langs:
            raise HTTPException(
                status_code=400,
                detail=f"Unsupported language. Supported: {', '.join(supported_langs)}"
            )

        # Save uploaded file temporarily
        with tempfile.NamedTemporaryFile(delete=False, suffix=".webm") as temp_file:
            content = await audio.read()
            temp_file.write(content)
            temp_path = temp_file.name

        try:
            # Perform speech recognition
            result = await speech_recognizer.transcribe(
                audio_path=temp_path,
                language=language
            )

            return SpeechResponse(
                text=result['text'],
                language=language,
                confidence=result.get('confidence')
            )

        finally:
            # Clean up temporary file
            if os.path.exists(temp_path):
                os.remove(temp_path)

    except Exception as e:
        logger.error(f"Speech-to-text error: {str(e)}")
        raise HTTPException(status_code=500, detail=f"Speech recognition failed: {str(e)}")
