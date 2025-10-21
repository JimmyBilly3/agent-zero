from fastapi import FastAPI, UploadFile, File, Form, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import StreamingResponse
from pydantic import BaseModel
from typing import Optional
import os
from dotenv import load_dotenv
import logging

from routes import translation, speech, tts, ocr

# Load environment variables
load_dotenv()

# Configure logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

# Initialize FastAPI app
app = FastAPI(
    title="靈語殿 API",
    description="The Palace of Living Words - Translation API",
    version="1.0.0"
)

# Configure CORS
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # In production, specify exact origins
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Include routers
app.include_router(translation.router, prefix="/api", tags=["Translation"])
app.include_router(speech.router, prefix="/api", tags=["Speech"])
app.include_router(tts.router, prefix="/api", tags=["Text-to-Speech"])
app.include_router(ocr.router, prefix="/api", tags=["OCR"])

@app.get("/")
async def root():
    return {
        "message": "Welcome to 靈語殿 - The Palace of Living Words",
        "status": "online",
        "version": "1.0.0",
        "endpoints": {
            "translate": "/api/translate",
            "speech": "/api/speech",
            "tts": "/api/tts",
            "ocr": "/api/ocr"
        }
    }

@app.get("/health")
async def health_check():
    return {
        "status": "healthy",
        "service": "靈語殿 Translation API"
    }

if __name__ == "__main__":
    import uvicorn
    host = os.getenv("HOST", "0.0.0.0")
    port = int(os.getenv("PORT", 8000))
    debug = os.getenv("DEBUG", "True").lower() == "true"

    logger.info(f"Starting 靈語殿 API on {host}:{port}")
    uvicorn.run(
        "main:app",
        host=host,
        port=port,
        reload=debug
    )
