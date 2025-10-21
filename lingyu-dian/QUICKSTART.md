# 🚀 Quick Start Guide

Get 靈語殿 running in 5 minutes!

## Step 1: Install Prerequisites

- **Node.js 18+**: https://nodejs.org/
- **Python 3.9+**: https://www.python.org/

## Step 2: Clone & Setup

```bash
# Clone the repository
git clone https://github.com/yourusername/lingyu-dian.git
cd lingyu-dian
```

## Step 3: Start Backend

```bash
cd backend
python -m venv venv

# Activate virtual environment
# Windows:
venv\Scripts\activate
# macOS/Linux:
source venv/bin/activate

# Install dependencies
pip install -r requirements.txt

# Create .env file (optional - will work without API keys in offline mode)
cp .env.example .env

# Start server
python main.py
```

**Backend running at:** http://localhost:8000

## Step 4: Start Frontend

Open a new terminal:

```bash
cd frontend
npm install
npm run dev
```

**Frontend running at:** http://localhost:3000

## Step 5: Open Your Browser

Navigate to: **http://localhost:3000**

🎉 **You're ready to translate!**

---

## First Translation

1. Select languages (e.g., English → Chinese)
2. Choose tone (Casual, Formal, or Classical)
3. Type or speak your text
4. Click "翻譯 Translate"

---

## Need API Keys?

For best results, add API keys in `backend/.env`:

```env
OPENAI_API_KEY=sk-your-key-here
```

Get your key at: https://platform.openai.com/api-keys

Without API keys, the app works in offline mode with fallback translation.

---

## Troubleshooting

**Backend won't start:**
- Make sure Python 3.9+ is installed
- Check that port 8000 is available

**Frontend won't start:**
- Make sure Node.js 18+ is installed
- Try: `rm -rf node_modules && npm install`

**Translation not working:**
- Check backend is running at http://localhost:8000
- Try offline mode in Settings

---

## Next Steps

- 📖 Read the full [README.md](README.md)
- ⚙️ Configure settings in the app
- 🎤 Try voice input
- 📷 Test camera OCR
- 📚 View your translation history

Enjoy translating! 靈語殿 welcomes you. ✨
