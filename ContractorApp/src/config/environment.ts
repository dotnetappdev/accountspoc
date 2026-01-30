/**
 * Environment Configuration
 * 
 * This file manages environment variables and configuration.
 * 
 * Note: The .env file is not automatically loaded in Expo/React Native.
 * To use environment variables from .env, you would need to:
 * 1. Install: npm install react-native-dotenv
 * 2. Configure babel.config.js to use the plugin
 * 
 * For this app, users configure settings via the Settings screen,
 * which stores values in SQLite. These defaults serve as fallbacks.
 */

// Default configuration
const DEFAULT_CONFIG = {
  API_URL: 'http://localhost:5001/api',
  OFFLINE_FIRST: true,
  WIFI_ONLY_SYNC: true,
};

/**
 * Get configuration value
 * Priority: Environment variable > Default value
 * 
 * Note: Currently returns defaults only. To enable .env file support,
 * install and configure react-native-dotenv or expo-constants.
 */
export const getConfig = (key: keyof typeof DEFAULT_CONFIG): string | boolean => {
  // In React Native/Expo, you can use expo-constants or react-native-dotenv for env vars
  // For this app, users configure via Settings screen which overrides these defaults
  return DEFAULT_CONFIG[key];
};

export const config = {
  apiUrl: getConfig('API_URL') as string,
  offlineFirst: getConfig('OFFLINE_FIRST') as boolean,
  wifiOnlySync: getConfig('WIFI_ONLY_SYNC') as boolean,
};

export default config;
