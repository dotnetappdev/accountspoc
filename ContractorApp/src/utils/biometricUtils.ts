import * as LocalAuthentication from 'expo-local-authentication';

/**
 * Check if biometric authentication is available on the device
 */
export const isBiometricAvailable = async (): Promise<boolean> => {
  try {
    const compatible = await LocalAuthentication.hasHardwareAsync();
    const enrolled = await LocalAuthentication.isEnrolledAsync();
    return compatible && enrolled;
  } catch (error) {
    console.error('Error checking biometric availability:', error);
    return false;
  }
};

/**
 * Get the type of biometric authentication available
 * Returns: 'Face ID', 'Touch ID', 'Biometric', or null
 */
export const getBiometricType = async (): Promise<string | null> => {
  try {
    const types = await LocalAuthentication.supportedAuthenticationTypesAsync();
    
    if (types.includes(LocalAuthentication.AuthenticationType.FACIAL_RECOGNITION)) {
      return 'Face ID';
    } else if (types.includes(LocalAuthentication.AuthenticationType.FINGERPRINT)) {
      return 'Touch ID';
    } else if (types.includes(LocalAuthentication.AuthenticationType.IRIS)) {
      return 'Iris';
    } else if (types.length > 0) {
      return 'Biometric';
    }
    
    return null;
  } catch (error) {
    console.error('Error getting biometric type:', error);
    return null;
  }
};

/**
 * Authenticate the user using biometrics
 */
export const authenticateWithBiometrics = async (
  promptMessage: string = 'Authenticate to continue'
): Promise<{ success: boolean; error?: string }> => {
  try {
    const available = await isBiometricAvailable();
    
    if (!available) {
      return {
        success: false,
        error: 'Biometric authentication is not available or not enrolled on this device',
      };
    }
    
    const result = await LocalAuthentication.authenticateAsync({
      promptMessage,
      cancelLabel: 'Cancel',
      disableDeviceFallback: false, // Allow PIN/password as fallback
      fallbackLabel: 'Use Passcode',
    });
    
    if (result.success) {
      return { success: true };
    } else {
      return {
        success: false,
        error: result.error || 'Authentication failed',
      };
    }
  } catch (error: any) {
    console.error('Biometric authentication error:', error);
    return {
      success: false,
      error: error.message || 'An error occurred during authentication',
    };
  }
};

/**
 * Get a user-friendly description of available biometric methods
 */
export const getBiometricDescription = async (): Promise<string> => {
  const type = await getBiometricType();
  const available = await isBiometricAvailable();
  
  if (!available) {
    return 'Not available or not enrolled';
  }
  
  switch (type) {
    case 'Face ID':
      return 'Face ID is available';
    case 'Touch ID':
      return 'Touch ID is available';
    case 'Iris':
      return 'Iris recognition is available';
    default:
      return 'Biometric authentication is available';
  }
};
