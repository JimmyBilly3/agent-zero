# 靈語殿 (Ling Yu Dian) - The Palace of Living Words

<div align="center">

![Version](https://img.shields.io/badge/version-1.0.0-gold)
![License](https://img.shields.io/badge/license-MIT-crimson)
![React](https://img.shields.io/badge/React-18.2-blue)
![FastAPI](https://img.shields.io/badge/FastAPI-0.104-green)

**A real-time multilingual translator featuring text, speech, and camera translation**

*Chinese ↔ English ↔ Thai*

</div>

---

## ✨ Features

### 🌍 **Multi-Language Support**
- **Languages:** English, Chinese (中文), Thai (ไทย)
- **Bidirectional translation** between any language pair
- Auto-detect source language (coming soon)

### 🎯 **Translation Modes**
- **Text Translation** - Instant and accurate text translation
- **Speech-to-Text** - Voice input with Whisper AI
- **Text-to-Speech** - Natural voice output in multiple languages
- **Camera/OCR** - Extract and translate text from images
- **Tone Selector** - Choose from Formal, Casual, or Classical styles

### 🌐 **Online & Offline Modes**
- **Online Mode** - High-accuracy translation using OpenAI GPT-4 or Anthropic Claude
- **Offline Mode** - Local translation models for privacy and offline use
- **Auto-fallback** - Seamlessly switches to offline when network is unavailable

### 💎 **Beautiful UI**
- **Gothic-Chinese Design** - Elegant fusion of ancient and modern aesthetics
- **Dark Theme** - Eye-friendly black velvet with gold accents
- **Smooth Animations** - Powered by Framer Motion
- **Responsive** - Works on desktop and mobile devices

### 📊 **Additional Features**
- **Translation History** - Save and search past translations
- **Settings Panel** - Customize language pairs, voice, and preferences
- **Overlay Mode** - Floating translator window (coming soon)
- **Data Privacy** - All data stored locally on your device

---

## 🚀 Quick Start

### Prerequisites

Before you begin, ensure you have the following installed:

- **Node.js** (v18 or higher) - [Download](https://nodejs.org/)
- **Python** (v3.9 or higher) - [Download](https://www.python.org/)
- **npm** or **yarn** - Comes with Node.js
- **pip** - Comes with Python

### Installation

#### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/lingyu-dian.git
cd lingyu-dian
```

#### 2. Set Up Frontend

```bash
cd frontend

# Install dependencies
npm install

# Create environment file
cp .env.example .env

# Edit .env and add your API keys (optional for basic usage)
```

#### 3. Set Up Backend

```bash
cd ../backend

# Create virtual environment (recommended)
python -m venv venv

# Activate virtual environment
# On Windows:
venv\Scripts\activate
# On macOS/Linux:
source venv/bin/activate

# Install dependencies
pip install -r requirements.txt

# Create environment file
cp .env.example .env

# Edit .env and add your API keys
```

---

## 🔑 API Keys Configuration

### Required for Online Mode

Edit `backend/.env` with your API keys:

```env
# At least one of these is required for online translation:
OPENAI_API_KEY=sk-your-openai-api-key-here
# OR
ANTHROPIC_API_KEY=sk-ant-your-anthropic-api-key-here

# Optional (for enhanced features):
GOOGLE_CLOUD_API_KEY=your-google-cloud-api-key
```

### How to Get API Keys

1. **OpenAI API Key**
   - Visit [OpenAI Platform](https://platform.openai.com/api-keys)
   - Sign up or log in
   - Create a new API key
   - Copy and paste into `.env`

2. **Anthropic API Key** (Alternative to OpenAI)
   - Visit [Anthropic Console](https://console.anthropic.com/)
   - Sign up or log in
   - Generate an API key
   - Copy and paste into `.env`

3. **Google Cloud API** (Optional - for TTS)
   - Visit [Google Cloud Console](https://console.cloud.google.com/)
   - Enable Text-to-Speech API
   - Create credentials
   - Copy and paste into `.env`

**Note:** The app will work in offline mode without API keys, but with limited translation accuracy.

---

## 🎮 Running the Application

### Option 1: Development Mode (Recommended)

**Terminal 1 - Backend:**
```bash
cd backend
source venv/bin/activate  # On Windows: venv\Scripts\activate
python main.py
```

The backend will start at `http://localhost:8000`

**Terminal 2 - Frontend:**
```bash
cd frontend
npm run dev
```

The frontend will start at `http://localhost:3000`

### Option 2: Using Scripts

Create these helper scripts in the root directory:

**`start-backend.sh` (macOS/Linux):**
```bash
#!/bin/bash
cd backend
source venv/bin/activate
python main.py
```

**`start-backend.bat` (Windows):**
```batch
@echo off
cd backend
call venv\Scripts\activate
python main.py
```

**`start-frontend.sh` (macOS/Linux):**
```bash
#!/bin/bash
cd frontend
npm run dev
```

**`start-frontend.bat` (Windows):**
```batch
@echo off
cd frontend
npm run dev
```

Make scripts executable:
```bash
chmod +x start-backend.sh start-frontend.sh
```

---

## 📦 Offline Model Setup

For offline translation without internet connection:

### Download Offline Models

```bash
cd models

# Download Whisper Tiny model (for speech recognition)
python -c "import whisper; whisper.load_model('base')"

# Download MarianMT models (for translation)
python -c "from transformers import MarianMTModel, MarianTokenizer; \
MarianTokenizer.from_pretrained('Helsinki-NLP/opus-mt-en-zh'); \
MarianMTModel.from_pretrained('Helsinki-NLP/opus-mt-en-zh')"
```

### Enable Offline Mode

In `backend/.env`:
```env
ENABLE_OFFLINE_MODE=True
DEFAULT_OFFLINE=True
```

Or toggle in the Settings panel in the app.

---

## 🏗️ Project Structure

```
lingyu-dian/
├── frontend/                 # React + Vite frontend
│   ├── src/
│   │   ├── components/      # Reusable UI components
│   │   │   ├── Layout.jsx
│   │   │   ├── LanguageSelector.jsx
│   │   │   ├── ToneSelector.jsx
│   │   │   └── CameraModal.jsx
│   │   ├── pages/           # Main pages
│   │   │   ├── Translate.jsx
│   │   │   ├── History.jsx
│   │   │   └── Settings.jsx
│   │   ├── hooks/           # Custom React hooks
│   │   │   ├── useAudioRecorder.js
│   │   │   └── useCamera.js
│   │   ├── utils/           # Utility functions
│   │   │   ├── api.js
│   │   │   ├── storage.js
│   │   │   └── cn.js
│   │   ├── store/           # State management
│   │   │   └── useStore.js
│   │   ├── App.jsx
│   │   ├── main.jsx
│   │   └── index.css
│   ├── public/
│   ├── package.json
│   ├── vite.config.js
│   └── tailwind.config.js
│
├── backend/                  # FastAPI backend
│   ├── routes/              # API routes
│   │   ├── translation.py
│   │   ├── speech.py
│   │   ├── tts.py
│   │   └── ocr.py
│   ├── models/              # AI models
│   │   ├── translator.py
│   │   ├── speech_recognizer.py
│   │   ├── text_to_speech.py
│   │   └── ocr_engine.py
│   ├── utils/
│   ├── main.py
│   └── requirements.txt
│
├── models/                   # Offline models storage
│   ├── whisper_tiny/
│   └── marianmt/
│
└── README.md
```

---

## 🎨 UI Design

### Color Palette

- **Black Velvet** `#0E0E0E` - Main background
- **Crimson** `#660000` - Accents and buttons
- **Gold** `#D4AF37` - Highlights and text
- **Misty Pink** `#E5A5C7` - Floating effects
- **Silver** `#C0C0C0` - Secondary text

### Typography

- **Headers:** Cinzel (Gothic serif)
- **Chinese Text:** Noto Serif SC
- **Body:** System fonts

---

## 🛠️ API Endpoints

### Translation
```http
POST /api/translate
Content-Type: application/json

{
  "text": "Hello, world!",
  "source_lang": "en",
  "target_lang": "zh",
  "tone": "casual",
  "offline": false
}
```

### Speech-to-Text
```http
POST /api/speech
Content-Type: multipart/form-data

audio: <audio-file>
language: "en"
```

### Text-to-Speech
```http
POST /api/tts
Content-Type: application/json

{
  "text": "Hello, world!",
  "language": "en",
  "voice": "female"
}
```

### OCR
```http
POST /api/ocr
Content-Type: multipart/form-data

image: <image-file>
```

---

## 🧪 Testing

### Test the Backend

```bash
cd backend
pytest tests/
```

### Test Translation Endpoint

```bash
curl -X POST http://localhost:8000/api/translate \
  -H "Content-Type: application/json" \
  -d '{
    "text": "Hello",
    "source_lang": "en",
    "target_lang": "zh",
    "tone": "casual"
  }'
```

---

## 🚢 Deployment

### Deploy Frontend to Vercel

```bash
cd frontend

# Install Vercel CLI
npm i -g vercel

# Deploy
vercel
```

### Deploy Backend to Render

1. Create a `render.yaml`:

```yaml
services:
  - type: web
    name: lingyu-dian-api
    env: python
    buildCommand: pip install -r requirements.txt
    startCommand: uvicorn main:app --host 0.0.0.0 --port $PORT
    envVars:
      - key: OPENAI_API_KEY
        sync: false
      - key: ANTHROPIC_API_KEY
        sync: false
```

2. Push to GitHub
3. Connect to Render.com
4. Deploy

### Deploy to HuggingFace Spaces

Create `app.py` in root:

```python
import gradio as gr
# Add Gradio interface here
```

Push to HuggingFace Spaces repository.

---

## 🔧 Troubleshooting

### Frontend Issues

**Port 3000 already in use:**
```bash
# Change port in vite.config.js
server: {
  port: 3001
}
```

**Dependencies not installing:**
```bash
# Clear cache and reinstall
rm -rf node_modules package-lock.json
npm install
```

### Backend Issues

**Port 8000 already in use:**
```bash
# Change port in .env
PORT=8001
```

**Module not found:**
```bash
# Ensure virtual environment is activated
source venv/bin/activate
pip install -r requirements.txt
```

**API key errors:**
- Check that `.env` file exists in `backend/` directory
- Verify API keys are correct (no extra spaces)
- Ensure `.env` is not in `.gitignore`

### Camera/Microphone Access

**Permission denied:**
- Check browser settings
- Ensure HTTPS or localhost
- Grant camera/microphone permissions

---

## 📚 Additional Resources

- [FastAPI Documentation](https://fastapi.tiangolo.com/)
- [React Documentation](https://react.dev/)
- [Vite Documentation](https://vitejs.dev/)
- [Tailwind CSS](https://tailwindcss.com/)
- [Framer Motion](https://www.framer.com/motion/)
- [OpenAI API](https://platform.openai.com/docs)
- [Whisper AI](https://github.com/openai/whisper)

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License.

---

## 🙏 Acknowledgments

- OpenAI for Whisper and GPT models
- Anthropic for Claude API
- Helsinki NLP for MarianMT models
- Edge-TTS for text-to-speech
- Tesseract for OCR
- All open-source contributors

---

## 📧 Contact

For questions or support, please open an issue on GitHub.

---

<div align="center">

**靈語殿 - 跨越語言的宮殿**

*The Palace of Living Words - Bridging Languages Through Ancient Elegance*

Made with ❤️ and ✨ by AI

</div>
