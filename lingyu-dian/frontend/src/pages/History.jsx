import React, { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { Trash2, Search, Clock, ArrowRight, AlertCircle } from 'lucide-react';
import { getTranslationHistory, deleteTranslation, clearTranslationHistory } from '../utils/storage';

const History = () => {
  const [history, setHistory] = useState([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    loadHistory();
  }, []);

  const loadHistory = async () => {
    try {
      const data = await getTranslationHistory();
      setHistory(data);
    } catch (error) {
      console.error('Error loading history:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async (id) => {
    try {
      await deleteTranslation(id);
      setHistory(history.filter(item => item.id !== id));
    } catch (error) {
      console.error('Error deleting translation:', error);
    }
  };

  const handleClearAll = async () => {
    if (window.confirm('Are you sure you want to clear all history?')) {
      try {
        await clearTranslationHistory();
        setHistory([]);
      } catch (error) {
        console.error('Error clearing history:', error);
      }
    }
  };

  const filteredHistory = history.filter(item =>
    item.sourceText.toLowerCase().includes(searchQuery.toLowerCase()) ||
    item.translatedText.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('en-US', {
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    }).format(date);
  };

  const getLangName = (code) => {
    const names = { en: 'EN', zh: '中文', th: 'ไทย' };
    return names[code] || code.toUpperCase();
  };

  return (
    <div className="max-w-4xl mx-auto">
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        className="gothic-card p-6 md:p-8"
      >
        {/* Header */}
        <div className="flex flex-col md:flex-row justify-between items-start md:items-center mb-6 gap-4">
          <h2 className="text-2xl md:text-3xl font-chinese text-gold glow-text">
            歷史記錄 Translation History
          </h2>
          {history.length > 0 && (
            <motion.button
              onClick={handleClearAll}
              whileHover={{ scale: 1.05 }}
              whileTap={{ scale: 0.95 }}
              className="px-4 py-2 bg-crimson/20 border border-crimson/50 rounded-lg text-crimson hover:bg-crimson/30 transition-all text-sm"
            >
              <Trash2 className="w-4 h-4 inline mr-2" />
              Clear All
            </motion.button>
          )}
        </div>

        {/* Search Bar */}
        <div className="relative mb-6">
          <Search className="absolute left-4 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gold/50" />
          <input
            type="text"
            placeholder="Search translations..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="gothic-input w-full pl-12"
          />
        </div>

        {/* History List */}
        <div className="space-y-4">
          {isLoading ? (
            <div className="text-center py-12">
              <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-gold mx-auto"></div>
              <p className="text-silver/60 mt-4">Loading history...</p>
            </div>
          ) : filteredHistory.length === 0 ? (
            <div className="text-center py-12">
              <AlertCircle className="w-16 h-16 text-gold/30 mx-auto mb-4" />
              <p className="text-silver/60">
                {searchQuery ? 'No translations found' : 'No translation history yet'}
              </p>
            </div>
          ) : (
            <AnimatePresence>
              {filteredHistory.map((item, index) => (
                <motion.div
                  key={item.id}
                  initial={{ opacity: 0, x: -20 }}
                  animate={{ opacity: 1, x: 0 }}
                  exit={{ opacity: 0, x: 20 }}
                  transition={{ delay: index * 0.05 }}
                  className="bg-black-velvet/50 border border-gold/20 rounded-lg p-4 hover:border-gold/40 transition-all group"
                >
                  <div className="flex justify-between items-start mb-3">
                    <div className="flex items-center space-x-2 text-xs text-silver/60">
                      <Clock className="w-3 h-3" />
                      <span>{formatDate(item.timestamp)}</span>
                      <span className="text-gold/60">•</span>
                      <span className="px-2 py-0.5 bg-crimson/20 border border-crimson/30 rounded text-crimson">
                        {item.tone}
                      </span>
                    </div>
                    <motion.button
                      onClick={() => handleDelete(item.id)}
                      whileHover={{ scale: 1.1 }}
                      whileTap={{ scale: 0.9 }}
                      className="opacity-0 group-hover:opacity-100 transition-opacity p-2 hover:bg-crimson/20 rounded-lg"
                    >
                      <Trash2 className="w-4 h-4 text-crimson" />
                    </motion.button>
                  </div>

                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                      <div className="text-xs text-gold/70 mb-1 font-gothic">
                        {getLangName(item.sourceLang)}
                      </div>
                      <p className="text-silver text-sm leading-relaxed">
                        {item.sourceText}
                      </p>
                    </div>

                    <div>
                      <div className="text-xs text-gold/70 mb-1 font-gothic flex items-center">
                        <ArrowRight className="w-3 h-3 mr-1" />
                        {getLangName(item.targetLang)}
                      </div>
                      <p className="text-silver text-sm leading-relaxed">
                        {item.translatedText}
                      </p>
                    </div>
                  </div>
                </motion.div>
              ))}
            </AnimatePresence>
          )}
        </div>

        {/* Stats */}
        {history.length > 0 && (
          <div className="mt-6 pt-4 border-t border-gold/10 text-center text-sm text-silver/60">
            Total translations: <span className="text-gold font-semibold">{history.length}</span>
          </div>
        )}
      </motion.div>
    </div>
  );
};

export default History;
