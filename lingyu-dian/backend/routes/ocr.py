from fastapi import APIRouter, UploadFile, File, HTTPException
from pydantic import BaseModel
from typing import Optional
import os
import tempfile
import logging

from models.ocr_engine import OCREngine

logger = logging.getLogger(__name__)
router = APIRouter()

# Initialize OCR engine
ocr_engine = OCREngine()

class OCRResponse(BaseModel):
    text: str
    confidence: Optional[float] = None
    detected_languages: Optional[list] = None

@router.post("/ocr", response_model=OCRResponse)
async def extract_text_from_image(image: UploadFile = File(...)):
    """
    Extract text from image using OCR.

    Supports:
    - Image formats: jpg, jpeg, png, webp
    - Languages: English, Chinese, Thai
    """
    try:
        logger.info(f"OCR request for image: {image.filename}")

        # Validate file type
        allowed_types = ['image/jpeg', 'image/png', 'image/webp', 'image/jpg']
        if image.content_type not in allowed_types:
            raise HTTPException(
                status_code=400,
                detail=f"Unsupported file type. Allowed: {', '.join(allowed_types)}"
            )

        # Save uploaded file temporarily
        with tempfile.NamedTemporaryFile(delete=False, suffix=".jpg") as temp_file:
            content = await image.read()
            temp_file.write(content)
            temp_path = temp_file.name

        try:
            # Perform OCR
            result = await ocr_engine.extract_text(image_path=temp_path)

            return OCRResponse(
                text=result['text'],
                confidence=result.get('confidence'),
                detected_languages=result.get('languages')
            )

        finally:
            # Clean up temporary file
            if os.path.exists(temp_path):
                os.remove(temp_path)

    except Exception as e:
        logger.error(f"OCR error: {str(e)}")
        raise HTTPException(status_code=500, detail=f"OCR failed: {str(e)}")
