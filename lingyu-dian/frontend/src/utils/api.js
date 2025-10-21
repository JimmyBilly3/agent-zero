import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Network status checker
export const checkOnlineStatus = () => {
  return navigator.onLine;
};

// Translation API
export const translateText = async (text, sourceLang, targetLang, tone = 'casual', offline = false) => {
  try {
    const response = await api.post('/api/translate', {
      text,
      source_lang: sourceLang,
      target_lang: targetLang,
      tone,
      offline,
    });
    return response.data;
  } catch (error) {
    console.error('Translation error:', error);
    throw error;
  }
};

// Speech to Text API
export const speechToText = async (audioBlob, language = 'en') => {
  try {
    const formData = new FormData();
    formData.append('audio', audioBlob);
    formData.append('language', language);

    const response = await api.post('/api/speech', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  } catch (error) {
    console.error('Speech to text error:', error);
    throw error;
  }
};

// Text to Speech API
export const textToSpeech = async (text, language = 'en', voice = 'default') => {
  try {
    const response = await api.post('/api/tts', {
      text,
      language,
      voice,
    }, {
      responseType: 'blob',
    });
    return response.data;
  } catch (error) {
    console.error('Text to speech error:', error);
    throw error;
  }
};

// OCR API
export const ocrImage = async (imageBlob) => {
  try {
    const formData = new FormData();
    formData.append('image', imageBlob);

    const response = await api.post('/api/ocr', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  } catch (error) {
    console.error('OCR error:', error);
    throw error;
  }
};

export default api;
