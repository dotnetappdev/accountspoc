/**
 * Environment Configuration
 * 
 * This file manages environment variables and configuration.
 * Uses .env file for configuration when available.
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
 */
export const getConfig = (key: keyof typeof DEFAULT_CONFIG): string | boolean => {
  // In React Native/Expo, you can use expo-constants for env vars
  // For now, return defaults - can be overridden in settings
  return DEFAULT_CONFIG[key];
};

export const config = {
  apiUrl: getConfig('API_URL') as string,
  offlineFirst: getConfig('OFFLINE_FIRST') as boolean,
  wifiOnlySync: getConfig('WIFI_ONLY_SYNC') as boolean,
};

export default config;
