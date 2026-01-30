import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { Ionicons } from '@expo/vector-icons';

// Import screens (we'll create these next)
import HomeScreen from '../screens/HomeScreen';
import SalesOrdersListScreen from '../screens/SalesOrdersListScreen';
import SalesOrderFormScreen from '../screens/SalesOrderFormScreen';
import QuotesListScreen from '../screens/QuotesListScreen';
import QuoteFormScreen from '../screens/QuoteFormScreen';
import WorkOrdersListScreen from '../screens/WorkOrdersListScreen';
import WorkOrderFormScreen from '../screens/WorkOrderFormScreen';
import SettingsScreen from '../screens/SettingsScreen';

export type RootStackParamList = {
  MainTabs: undefined;
  SalesOrderForm: { id?: number };
  QuoteForm: { id?: number };
  WorkOrderForm: { id?: number };
};

export type TabParamList = {
  Home: undefined;
  SalesOrders: undefined;
  Quotes: undefined;
  WorkOrders: undefined;
  Settings: undefined;
};

const Tab = createBottomTabNavigator<TabParamList>();
const Stack = createNativeStackNavigator<RootStackParamList>();

const TabNavigator = () => {
  return (
    <Tab.Navigator
      screenOptions={({ route }) => ({
        tabBarIcon: ({ focused, color, size }) => {
          let iconName: keyof typeof Ionicons.glyphMap = 'home';

          if (route.name === 'Home') {
            iconName = focused ? 'home' : 'home-outline';
          } else if (route.name === 'SalesOrders') {
            iconName = focused ? 'cart' : 'cart-outline';
          } else if (route.name === 'Quotes') {
            iconName = focused ? 'document-text' : 'document-text-outline';
          } else if (route.name === 'WorkOrders') {
            iconName = focused ? 'build' : 'build-outline';
          } else if (route.name === 'Settings') {
            iconName = focused ? 'settings' : 'settings-outline';
          }

          return <Ionicons name={iconName} size={size} color={color} />;
        },
        tabBarActiveTintColor: '#007AFF',
        tabBarInactiveTintColor: 'gray',
        headerStyle: {
          backgroundColor: '#007AFF',
        },
        headerTintColor: '#fff',
        headerTitleStyle: {
          fontWeight: 'bold',
        },
      })}
    >
      <Tab.Screen 
        name="Home" 
        component={HomeScreen}
        options={{ title: 'Dashboard' }}
      />
      <Tab.Screen 
        name="SalesOrders" 
        component={SalesOrdersListScreen}
        options={{ title: 'Sales Orders' }}
      />
      <Tab.Screen 
        name="Quotes" 
        component={QuotesListScreen}
        options={{ title: 'Quotes' }}
      />
      <Tab.Screen 
        name="WorkOrders" 
        component={WorkOrdersListScreen}
        options={{ title: 'Work Orders' }}
      />
      <Tab.Screen 
        name="Settings" 
        component={SettingsScreen}
        options={{ title: 'Settings' }}
      />
    </Tab.Navigator>
  );
};

const Navigation = () => {
  return (
    <NavigationContainer>
      <Stack.Navigator>
        <Stack.Screen 
          name="MainTabs" 
          component={TabNavigator}
          options={{ headerShown: false }}
        />
        <Stack.Screen 
          name="SalesOrderForm" 
          component={SalesOrderFormScreen}
          options={{ title: 'Sales Order' }}
        />
        <Stack.Screen 
          name="QuoteForm" 
          component={QuoteFormScreen}
          options={{ title: 'Quote' }}
        />
        <Stack.Screen 
          name="WorkOrderForm" 
          component={WorkOrderFormScreen}
          options={{ title: 'Work Order' }}
        />
      </Stack.Navigator>
    </NavigationContainer>
  );
};

export default Navigation;
