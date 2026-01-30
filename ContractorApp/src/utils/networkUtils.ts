/**
 * Network Utilities
 * 
 * Provides network connectivity detection and WiFi checking
 */

import NetInfo, { NetInfoState } from '@react-native-community/netinfo';

/**
 * Check if device is connected to the internet
 */
export const isConnected = async (): Promise<boolean> => {
  const state = await NetInfo.fetch();
  return state.isConnected ?? false;
};

/**
 * Check if device is connected via WiFi
 */
export const isWiFiConnected = async (): Promise<boolean> => {
  const state = await NetInfo.fetch();
  return state.isConnected === true && state.type === 'wifi';
};

/**
 * Check if device is connected via cellular/mobile data
 */
export const isCellularConnected = async (): Promise<boolean> => {
  const state = await NetInfo.fetch();
  return state.isConnected === true && state.type === 'cellular';
};

/**
 * Get current network type
 */
export const getNetworkType = async (): Promise<string> => {
  const state = await NetInfo.fetch();
  return state.type;
};

/**
 * Subscribe to network state changes
 */
export const subscribeToNetworkChanges = (callback: (state: NetInfoState) => void) => {
  return NetInfo.addEventListener(callback);
};

/**
 * Check if sync should be allowed based on network conditions
 * @param wifiOnly - If true, only allow sync on WiFi
 */
export const shouldAllowSync = async (wifiOnly: boolean = true): Promise<boolean> => {
  const connected = await isConnected();
  
  if (!connected) {
    return false;
  }
  
  if (wifiOnly) {
    return await isWiFiConnected();
  }
  
  return true;
};
