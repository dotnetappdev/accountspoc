import React, { useEffect } from 'react';
import { StatusBar } from 'expo-status-bar';
import Navigation from './src/navigation/Navigation';
import { initDatabase } from './src/database/database';
import { ThemeProvider } from './src/contexts/ThemeContext';

export default function App() {
  useEffect(() => {
    // Initialize database when app starts
    initDatabase();
  }, []);

  return (
    <ThemeProvider>
      <Navigation />
      <StatusBar style="auto" />
    </ThemeProvider>
  );
}
