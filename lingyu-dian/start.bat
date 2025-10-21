@echo off
echo 🏯 Starting 靈語殿 - The Palace of Living Words...
echo.

:: Check if Python is installed
python --version >nul 2>&1
if errorlevel 1 (
    echo ❌ Python is not installed. Please install Python 3.9 or higher.
    pause
    exit /b 1
)

:: Check if Node.js is installed
node --version >nul 2>&1
if errorlevel 1 (
    echo ❌ Node.js is not installed. Please install Node.js 18 or higher.
    pause
    exit /b 1
)

echo ✅ Prerequisites check passed
echo.

:: Start backend
echo 🔧 Starting backend server...
cd backend

if not exist "venv\" (
    echo Creating virtual environment...
    python -m venv venv
)

call venv\Scripts\activate
pip install -q -r requirements.txt

start /B python main.py

cd ..

:: Start frontend
echo 🎨 Starting frontend server...
cd frontend

if not exist "node_modules\" (
    echo Installing frontend dependencies...
    npm install
)

start /B npm run dev

cd ..

echo.
echo ✨ 靈語殿 is running!
echo.
echo 📍 Frontend: http://localhost:3000
echo 📍 Backend:  http://localhost:8000
echo.
echo Press any key to stop all servers
pause >nul

taskkill /F /IM python.exe /T >nul 2>&1
taskkill /F /IM node.exe /T >nul 2>&1
