import React from 'react';
import { motion } from 'framer-motion';
import { Home, History, Settings, Sparkles } from 'lucide-react';
import useStore from '../store/useStore';

const Layout = ({ children }) => {
  const { activeTab, setActiveTab } = useStore();

  const navItems = [
    { id: 'translate', icon: Home, label: '翻譯 Translate' },
    { id: 'history', icon: History, label: '歷史 History' },
    { id: 'settings', icon: Settings, label: '設定 Settings' },
  ];

  return (
    <div className="min-h-screen relative overflow-hidden">
      {/* Mist effect */}
      <div className="mist-effect" />

      {/* Floating petals */}
      {[...Array(15)].map((_, i) => (
        <motion.div
          key={i}
          className="floating-petals"
          style={{
            left: `${Math.random() * 100}%`,
            top: `${Math.random() * 100}%`,
          }}
          animate={{
            y: [0, -100, 0],
            x: [0, Math.random() * 50 - 25, 0],
            opacity: [0.3, 0.6, 0.3],
          }}
          transition={{
            duration: 10 + Math.random() * 10,
            repeat: Infinity,
            delay: Math.random() * 5,
          }}
        />
      ))}

      {/* Header */}
      <header className="relative z-10 border-b border-gold/20 bg-black-velvet/80 backdrop-blur-md">
        <div className="container mx-auto px-4 py-6">
          <motion.div
            initial={{ opacity: 0, y: -20 }}
            animate={{ opacity: 1, y: 0 }}
            className="text-center"
          >
            <h1 className="text-4xl md:text-5xl font-bold font-chinese glow-text mb-2">
              靈語殿
            </h1>
            <p className="text-gold/80 text-sm md:text-base font-gothic tracking-wider">
              The Palace of Living Words
            </p>
          </motion.div>
        </div>
      </header>

      {/* Navigation */}
      <nav className="relative z-10 border-b border-gold/10 bg-black-velvet/60 backdrop-blur-sm">
        <div className="container mx-auto px-4">
          <div className="flex justify-center space-x-2 md:space-x-8">
            {navItems.map((item) => {
              const Icon = item.icon;
              const isActive = activeTab === item.id;

              return (
                <motion.button
                  key={item.id}
                  onClick={() => setActiveTab(item.id)}
                  className={`
                    relative px-4 md:px-6 py-4 flex items-center space-x-2 transition-all duration-300
                    ${isActive ? 'text-gold' : 'text-silver/60 hover:text-silver'}
                  `}
                  whileHover={{ scale: 1.05 }}
                  whileTap={{ scale: 0.95 }}
                >
                  <Icon className="w-5 h-5" />
                  <span className="hidden md:inline font-gothic text-sm">
                    {item.label}
                  </span>
                  {isActive && (
                    <motion.div
                      layoutId="activeTab"
                      className="absolute bottom-0 left-0 right-0 h-0.5 bg-gradient-to-r from-transparent via-gold to-transparent"
                      initial={false}
                      transition={{ type: 'spring', stiffness: 500, damping: 30 }}
                    />
                  )}
                </motion.button>
              );
            })}
          </div>
        </div>
      </nav>

      {/* Main content */}
      <main className="relative z-10 container mx-auto px-4 py-8">
        {children}
      </main>

      {/* Footer */}
      <footer className="relative z-10 mt-16 border-t border-gold/10 bg-black-velvet/60 backdrop-blur-sm">
        <div className="container mx-auto px-4 py-6">
          <div className="flex flex-col md:flex-row items-center justify-between text-xs md:text-sm text-silver/60">
            <div className="flex items-center space-x-2 mb-2 md:mb-0">
              <Sparkles className="w-4 h-4 text-gold" />
              <span className="font-chinese">靈語殿 - 跨越語言的宮殿</span>
            </div>
            <div className="flex space-x-4">
              <span className="text-crimson">黑絲絨 Black Velvet</span>
              <span className="text-gold">金 Gold</span>
              <span className="text-misty-pink">霧粉 Misty Pink</span>
            </div>
          </div>
        </div>
      </footer>
    </div>
  );
};

export default Layout;
