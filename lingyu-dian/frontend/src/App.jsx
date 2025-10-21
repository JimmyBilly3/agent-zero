import React, { useEffect } from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import Layout from './components/Layout';
import Translate from './pages/Translate';
import History from './pages/History';
import Settings from './pages/Settings';
import useStore from './store/useStore';

function App() {
  const { activeTab, loadSettings } = useStore();

  useEffect(() => {
    loadSettings();
  }, [loadSettings]);

  const renderPage = () => {
    switch (activeTab) {
      case 'translate':
        return <Translate />;
      case 'history':
        return <History />;
      case 'settings':
        return <Settings />;
      default:
        return <Translate />;
    }
  };

  return (
    <Router>
      <Layout>
        {renderPage()}
      </Layout>
    </Router>
  );
}

export default App;
