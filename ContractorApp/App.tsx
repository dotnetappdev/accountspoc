import React, { useEffect } from 'react';
import { StatusBar } from 'expo-status-bar';
import Navigation from './src/navigation/Navigation';
import { initDatabase } from './src/database/database';

export default function App() {
  useEffect(() => {
    // Initialize database when app starts
    initDatabase();
  }, []);

  return (
    <>
      <Navigation />
      <StatusBar style="auto" />
    </>
  );
}
