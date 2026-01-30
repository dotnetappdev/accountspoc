import React, { useState, useEffect, useContext } from 'react';
import {
  View,
  Text,
  TextInput,
  StyleSheet,
  TouchableOpacity,
  Alert,
  Switch,
  ScrollView,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import db, { seedTestData } from '../database/database';
import apiService from '../services/apiService';
import { ThemeContext } from '../contexts/ThemeContext';
import { isWiFiConnected, isCellularConnected } from '../utils/networkUtils';
import { 
  isBiometricAvailable, 
  getBiometricDescription, 
  authenticateWithBiometrics 
} from '../utils/biometricUtils';

const SettingsScreen = () => {
  const [apiUrl, setApiUrl] = useState('http://localhost:5001/api');
  const [syncEnabled, setSyncEnabled] = useState(true);
  const [wifiOnlySync, setWifiOnlySync] = useState(true);
  const [biometricEnabled, setBiometricEnabled] = useState(false);
  const [biometricAvailable, setBiometricAvailable] = useState(false);
  const [biometricType, setBiometricType] = useState<string>('');
  const [lastSync, setLastSync] = useState<string | null>(null);
  const [syncing, setSyncing] = useState(false);
  const [networkStatus, setNetworkStatus] = useState<string>('Checking...');
  const { theme, setTheme } = useContext(ThemeContext);

  useEffect(() => {
    loadSettings();
    checkNetworkStatus();
    checkBiometricAvailability();
  }, []);

  const checkBiometricAvailability = async () => {
    const available = await isBiometricAvailable();
    setBiometricAvailable(available);
    
    if (available) {
      const description = await getBiometricDescription();
      setBiometricType(description);
    } else {
      // If biometrics are no longer available, disable the setting
      setBiometricEnabled(false);
    }
  };

  const getBiometricLabel = (): string => {
    if (biometricType?.includes('Face')) return 'Face ID';
    if (biometricType?.includes('Touch')) return 'Touch ID';
    if (biometricType?.includes('Iris')) return 'Iris Recognition';
    return 'Biometric Auth';
  };

  const checkNetworkStatus = async () => {
    const wifi = await isWiFiConnected();
    const cellular = await isCellularConnected();
    
    if (wifi) {
      setNetworkStatus('Connected to WiFi');
    } else if (cellular) {
      setNetworkStatus('Connected to Mobile Data');
    } else {
      setNetworkStatus('Offline');
    }
  };

  const loadSettings = () => {
    try {
      const settings: Record<string, string> = {};
      const rows = db.getAllSync('SELECT key, value FROM settings') as any[];
      rows.forEach(row => {
        settings[row.key] = row.value;
      });
      
      setApiUrl(settings.apiUrl || 'http://localhost:5001/api');
      setSyncEnabled(settings.syncEnabled === '1');
      setWifiOnlySync(settings.wifiOnlySync === '1');
      // Biometric setting will be validated against availability in checkBiometricAvailability
      setBiometricEnabled(settings.biometricEnabled === '1');
      setLastSync(settings.lastSync || null);
    } catch (error) {
      console.error('Error loading settings:', error);
    }
  };

  const saveSettings = () => {
    try {
      // Use INSERT OR REPLACE for atomic operations
      db.runSync('INSERT OR REPLACE INTO settings (key, value) VALUES (?, ?)', ['apiUrl', apiUrl]);
      db.runSync('INSERT OR REPLACE INTO settings (key, value) VALUES (?, ?)', ['syncEnabled', syncEnabled ? '1' : '0']);
      db.runSync('INSERT OR REPLACE INTO settings (key, value) VALUES (?, ?)', ['wifiOnlySync', wifiOnlySync ? '1' : '0']);
      db.runSync('INSERT OR REPLACE INTO settings (key, value) VALUES (?, ?)', ['biometricEnabled', biometricEnabled ? '1' : '0']);
      
      apiService.updateBaseURL(apiUrl);
      
      Alert.alert('Success', 'Settings saved successfully');
    } catch (error) {
      console.error('Error saving settings:', error);
      Alert.alert('Error', 'Failed to save settings');
    }
  };

  const handleBiometricToggle = async (value: boolean) => {
    if (value) {
      // When enabling, test biometric authentication
      const result = await authenticateWithBiometrics('Authenticate to enable biometric security');
      
      if (result.success) {
        setBiometricEnabled(true);
        // Auto-save after successful enable
        try {
          db.runSync('INSERT OR REPLACE INTO settings (key, value) VALUES (?, ?)', ['biometricEnabled', '1']);
          Alert.alert('Success', 'Biometric authentication enabled');
        } catch (error) {
          console.error('Error saving biometric setting:', error);
          Alert.alert('Error', 'Biometric enabled but could not save setting');
        }
      } else {
        Alert.alert('Authentication Failed', result.error || 'Could not authenticate');
      }
    } else {
      // When disabling, require confirmation
      Alert.alert(
        'Disable Biometric Auth',
        'Are you sure you want to disable biometric authentication?',
        [
          { text: 'Cancel', style: 'cancel' },
          {
            text: 'Disable',
            style: 'destructive',
            onPress: () => {
              setBiometricEnabled(false);
              // Auto-save after disable
              try {
                db.runSync('INSERT OR REPLACE INTO settings (key, value) VALUES (?, ?)', ['biometricEnabled', '0']);
              } catch (error) {
                console.error('Error saving biometric setting:', error);
              }
            },
          },
        ]
      );
    }
  };

  const testBiometricAuth = async () => {
    const result = await authenticateWithBiometrics('Test biometric authentication');
    
    if (result.success) {
      Alert.alert('Success', 'Biometric authentication successful!');
    } else {
      Alert.alert('Failed', result.error || 'Authentication failed');
    }
  };

  const handleSeedData = () => {
    Alert.alert(
      'Seed Test Data',
      'This will clear existing data and add sample data. Continue?',
      [
        { text: 'Cancel', style: 'cancel' },
        {
          text: 'OK',
          onPress: () => {
            try {
              seedTestData();
              Alert.alert('Success', 'Test data seeded successfully');
            } catch (error) {
              console.error('Error seeding data:', error);
              Alert.alert('Error', 'Failed to seed test data');
            }
          },
        },
      ]
    );
  };

  const handleSyncToServer = async () => {
    if (!syncEnabled) {
      Alert.alert('Sync Disabled', 'Please enable sync in settings');
      return;
    }

    setSyncing(true);
    try {
      await apiService.syncToServer();
      loadSettings(); // Reload to get updated lastSync
      Alert.alert('Success', 'Data synced to server successfully');
    } catch (error: any) {
      console.error('Sync error:', error);
      Alert.alert('Sync Error', error.message || 'Failed to sync data');
    } finally {
      setSyncing(false);
    }
  };

  const handleSyncFromServer = async () => {
    if (!syncEnabled) {
      Alert.alert('Sync Disabled', 'Please enable sync in settings');
      return;
    }

    setSyncing(true);
    try {
      await apiService.syncFromServer();
      loadSettings();
      Alert.alert('Success', 'Data synced from server successfully');
    } catch (error: any) {
      console.error('Sync error:', error);
      Alert.alert('Sync Error', error.message || 'Failed to sync data');
    } finally {
      setSyncing(false);
    }
  };

  const clearAllData = () => {
    Alert.alert(
      'Clear All Data',
      'This will delete all local data. This action cannot be undone. Continue?',
      [
        { text: 'Cancel', style: 'cancel' },
        {
          text: 'Delete',
          style: 'destructive',
          onPress: () => {
            try {
              db.execSync('DELETE FROM sales_order_items');
              db.execSync('DELETE FROM sales_orders');
              db.execSync('DELETE FROM quote_items');
              db.execSync('DELETE FROM quotes');
              db.execSync('DELETE FROM work_order_tasks');
              db.execSync('DELETE FROM site_visit_signoffs');
              db.execSync('DELETE FROM work_orders');
              Alert.alert('Success', 'All data cleared');
            } catch (error) {
              console.error('Error clearing data:', error);
              Alert.alert('Error', 'Failed to clear data');
            }
          },
        },
      ]
    );
  };

  return (
    <ScrollView style={styles.container}>
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Network Status</Text>
        <View style={styles.infoBox}>
          <Ionicons name="wifi" size={20} color="#8E8E93" />
          <Text style={styles.infoText}>{networkStatus}</Text>
        </View>
        <Text style={styles.helpText}>
          App works fully offline. Sync only when needed.
        </Text>
      </View>

      <View style={styles.section}>
        <Text style={styles.sectionTitle}>API Configuration</Text>
        
        <Text style={styles.label}>API URL</Text>
        <TextInput
          style={styles.input}
          value={apiUrl}
          onChangeText={setApiUrl}
          placeholder="http://localhost:5001/api"
          autoCapitalize="none"
          autoCorrect={false}
        />
        <Text style={styles.helpText}>
          Configure your backend API URL. Leave as default for local development.
        </Text>
        
        <View style={styles.switchContainer}>
          <Text style={styles.label}>Enable Sync</Text>
          <Switch value={syncEnabled} onValueChange={setSyncEnabled} />
        </View>

        <View style={styles.switchContainer}>
          <View style={styles.switchLabelContainer}>
            <Text style={styles.label}>WiFi Only Sync</Text>
            <Text style={styles.helpText}>Only sync when connected to WiFi (recommended)</Text>
          </View>
          <Switch value={wifiOnlySync} onValueChange={setWifiOnlySync} />
        </View>

        <TouchableOpacity style={styles.button} onPress={saveSettings}>
          <Ionicons name="save" size={20} color="#fff" />
          <Text style={styles.buttonText}>Save Settings</Text>
        </TouchableOpacity>
      </View>

      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Appearance</Text>
        
        <View style={styles.themeButtons}>
          <TouchableOpacity 
            style={[styles.themeButton, theme === 'light' && styles.themeButtonActive]}
            onPress={() => setTheme('light')}
          >
            <Ionicons name="sunny" size={24} color={theme === 'light' ? '#007AFF' : '#8E8E93'} />
            <Text style={[styles.themeButtonText, theme === 'light' && styles.themeButtonTextActive]}>Light</Text>
          </TouchableOpacity>
          
          <TouchableOpacity 
            style={[styles.themeButton, theme === 'dark' && styles.themeButtonActive]}
            onPress={() => setTheme('dark')}
          >
            <Ionicons name="moon" size={24} color={theme === 'dark' ? '#007AFF' : '#8E8E93'} />
            <Text style={[styles.themeButtonText, theme === 'dark' && styles.themeButtonTextActive]}>Dark</Text>
          </TouchableOpacity>
          
          <TouchableOpacity 
            style={[styles.themeButton, theme === 'auto' && styles.themeButtonActive]}
            onPress={() => setTheme('auto')}
          >
            <Ionicons name="contrast" size={24} color={theme === 'auto' ? '#007AFF' : '#8E8E93'} />
            <Text style={[styles.themeButtonText, theme === 'auto' && styles.themeButtonTextActive]}>Auto</Text>
          </TouchableOpacity>
        </View>
      </View>

      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Security</Text>
        
        {biometricAvailable ? (
          <>
            <View style={styles.switchContainer}>
              <View style={styles.switchLabelContainer}>
                <Text style={styles.label}>Use {getBiometricLabel()}</Text>
                <Text style={styles.helpText}>{biometricType}</Text>
              </View>
              <Switch value={biometricEnabled} onValueChange={handleBiometricToggle} />
            </View>
            
            {biometricEnabled && (
              <TouchableOpacity style={styles.buttonSecondary} onPress={testBiometricAuth}>
                <Ionicons name="finger-print" size={20} color="#007AFF" />
                <Text style={styles.buttonTextSecondary}>Test Authentication</Text>
              </TouchableOpacity>
            )}
          </>
        ) : (
          <View style={styles.infoBox}>
            <Ionicons name="information-circle" size={20} color="#8E8E93" />
            <Text style={styles.infoText}>
              Biometric authentication not available on this device
            </Text>
          </View>
        )}
      </View>

      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Data Management</Text>
        
        <TouchableOpacity style={styles.buttonSecondary} onPress={handleSeedData}>
          <Ionicons name="flask" size={20} color="#007AFF" />
          <Text style={styles.buttonTextSecondary}>Seed Test Data</Text>
        </TouchableOpacity>

        <TouchableOpacity 
          style={[styles.buttonSecondary, syncing && styles.buttonDisabled]} 
          onPress={handleSyncToServer}
          disabled={syncing}
        >
          <Ionicons name="cloud-upload" size={20} color="#007AFF" />
          <Text style={styles.buttonTextSecondary}>
            {syncing ? 'Syncing...' : 'Sync to Server'}
          </Text>
        </TouchableOpacity>

        <TouchableOpacity 
          style={[styles.buttonSecondary, syncing && styles.buttonDisabled]} 
          onPress={handleSyncFromServer}
          disabled={syncing}
        >
          <Ionicons name="cloud-download" size={20} color="#007AFF" />
          <Text style={styles.buttonTextSecondary}>
            {syncing ? 'Syncing...' : 'Sync from Server'}
          </Text>
        </TouchableOpacity>

        <TouchableOpacity style={styles.buttonDanger} onPress={clearAllData}>
          <Ionicons name="trash" size={20} color="#fff" />
          <Text style={styles.buttonText}>Clear All Data</Text>
        </TouchableOpacity>
      </View>

      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Sync Status</Text>
        <View style={styles.infoBox}>
          <Ionicons name="time" size={20} color="#8E8E93" />
          <Text style={styles.infoText}>
            Last sync: {lastSync ? new Date(lastSync).toLocaleString() : 'Never'}
          </Text>
        </View>
      </View>

      <View style={styles.section}>
        <Text style={styles.sectionTitle}>About</Text>
        <Text style={styles.aboutText}>
          Contractor App v1.0{'\n'}
          Developed for AccountsPOC{'\n'}
          Supports Windows, Linux, iOS, and Android{'\n\n'}
          Offline-First Architecture{'\n'}
          All features work without internet connection
        </Text>
      </View>
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#F2F2F7',
  },
  section: {
    backgroundColor: '#fff',
    margin: 15,
    padding: 20,
    borderRadius: 12,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  sectionTitle: {
    fontSize: 20,
    fontWeight: '600',
    marginBottom: 15,
    color: '#000',
  },
  label: {
    fontSize: 14,
    fontWeight: '500',
    marginBottom: 8,
    color: '#000',
  },
  input: {
    borderWidth: 1,
    borderColor: '#ddd',
    borderRadius: 8,
    padding: 12,
    marginBottom: 15,
    fontSize: 14,
  },
  switchContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 15,
  },
  button: {
    backgroundColor: '#007AFF',
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    padding: 15,
    borderRadius: 8,
    marginTop: 10,
  },
  buttonSecondary: {
    backgroundColor: '#fff',
    borderWidth: 1,
    borderColor: '#007AFF',
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    padding: 15,
    borderRadius: 8,
    marginTop: 10,
  },
  buttonDanger: {
    backgroundColor: '#FF3B30',
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    padding: 15,
    borderRadius: 8,
    marginTop: 10,
  },
  buttonDisabled: {
    opacity: 0.5,
  },
  buttonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: '600',
    marginLeft: 8,
  },
  buttonTextSecondary: {
    color: '#007AFF',
    fontSize: 16,
    fontWeight: '600',
    marginLeft: 8,
  },
  infoBox: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: 12,
    backgroundColor: '#F2F2F7',
    borderRadius: 8,
  },
  infoText: {
    marginLeft: 10,
    fontSize: 14,
    color: '#8E8E93',
  },
  aboutText: {
    fontSize: 14,
    color: '#666',
    lineHeight: 22,
  },
  helpText: {
    fontSize: 12,
    color: '#8E8E93',
    marginBottom: 12,
    marginTop: -8,
  },
  themeButtons: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    gap: 10,
  },
  themeButton: {
    flex: 1,
    alignItems: 'center',
    padding: 15,
    borderRadius: 8,
    borderWidth: 2,
    borderColor: '#E5E5EA',
    backgroundColor: '#fff',
  },
  themeButtonActive: {
    borderColor: '#007AFF',
    backgroundColor: '#E3F2FD',
  },
  themeButtonText: {
    marginTop: 8,
    fontSize: 14,
    color: '#8E8E93',
    fontWeight: '500',
  },
  themeButtonTextActive: {
    color: '#007AFF',
    fontWeight: '600',
  },
  switchLabelContainer: {
    flex: 1,
  },
});

export default SettingsScreen;
