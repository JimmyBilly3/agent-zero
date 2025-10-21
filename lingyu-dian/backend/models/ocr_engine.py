import os
import logging
from typing import Dict, Any
import cv2
import numpy as np
from PIL import Image

logger = logging.getLogger(__name__)

class OCREngine:
    """
    Handles optical character recognition (OCR) from images.
    """

    def __init__(self):
        self.tesseract_available = False
        try:
            import pytesseract
            self.pytesseract = pytesseract
            self.tesseract_available = True
            logger.info("Tesseract OCR available")
        except Exception as e:
            logger.warning(f"Tesseract not available: {e}")

    async def extract_text(self, image_path: str) -> Dict[str, Any]:
        """
        Extract text from image using OCR.

        Args:
            image_path: Path to image file

        Returns:
            Dict with text, confidence, and detected languages
        """
        try:
            # Read and preprocess image
            image = cv2.imread(image_path)
            if image is None:
                raise Exception("Failed to read image")

            # Convert to grayscale
            gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

            # Apply thresholding to improve OCR accuracy
            _, thresh = cv2.threshold(gray, 0, 255, cv2.THRESH_BINARY + cv2.THRESH_OTSU)

            if self.tesseract_available:
                # Perform OCR with Tesseract
                # Support for English, Chinese, and Thai
                custom_config = r'--oem 3 --psm 6 -l eng+chi_sim+tha'

                # Extract text
                pil_image = Image.fromarray(thresh)
                text = self.pytesseract.image_to_string(pil_image, config=custom_config)

                # Get confidence
                data = self.pytesseract.image_to_data(pil_image, output_type=self.pytesseract.Output.DICT)
                confidences = [int(conf) for conf in data['conf'] if conf != '-1']
                avg_confidence = sum(confidences) / len(confidences) if confidences else 0

                logger.info(f"OCR successful, confidence: {avg_confidence}")

                return {
                    'text': text.strip(),
                    'confidence': avg_confidence / 100.0,
                    'languages': ['en', 'zh', 'th']
                }
            else:
                # Fallback: Return mock response
                logger.warning("Using fallback OCR")
                return {
                    'text': "[OCR text would appear here - Tesseract not available]",
                    'confidence': 0.5,
                    'languages': []
                }

        except Exception as e:
            logger.error(f"OCR error: {e}")
            raise Exception(f"OCR failed: {str(e)}")
