from fastapi import APIRouter, HTTPException
from pydantic import BaseModel
from typing import Optional
import os
import logging

from models.translator import Translator

logger = logging.getLogger(__name__)
router = APIRouter()

# Initialize translator
translator = Translator()

class TranslationRequest(BaseModel):
    text: str
    source_lang: str
    target_lang: str
    tone: Optional[str] = "casual"
    offline: Optional[bool] = False

class TranslationResponse(BaseModel):
    translated_text: str
    source_lang: str
    target_lang: str
    tone: str
    confidence: Optional[float] = None
    method: str  # "online" or "offline"

@router.post("/translate", response_model=TranslationResponse)
async def translate_text(request: TranslationRequest):
    """
    Translate text from source language to target language.

    Supports:
    - Languages: en (English), zh (Chinese), th (Thai)
    - Tones: formal, casual, classical
    - Online and offline modes
    """
    try:
        logger.info(f"Translation request: {request.source_lang} -> {request.target_lang}, offline={request.offline}")

        # Validate languages
        supported_langs = ['en', 'zh', 'th']
        if request.source_lang not in supported_langs or request.target_lang not in supported_langs:
            raise HTTPException(
                status_code=400,
                detail=f"Unsupported language. Supported: {', '.join(supported_langs)}"
            )

        if request.source_lang == request.target_lang:
            raise HTTPException(
                status_code=400,
                detail="Source and target languages must be different"
            )

        # Perform translation
        result = await translator.translate(
            text=request.text,
            source_lang=request.source_lang,
            target_lang=request.target_lang,
            tone=request.tone,
            offline=request.offline
        )

        return TranslationResponse(
            translated_text=result['translated_text'],
            source_lang=request.source_lang,
            target_lang=request.target_lang,
            tone=request.tone,
            confidence=result.get('confidence'),
            method=result.get('method', 'online')
        )

    except Exception as e:
        logger.error(f"Translation error: {str(e)}")
        raise HTTPException(status_code=500, detail=f"Translation failed: {str(e)}")
