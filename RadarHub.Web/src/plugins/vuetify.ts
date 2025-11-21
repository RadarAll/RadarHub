import 'vuetify/styles'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'

export default createVuetify({
  components,
  directives,
  theme: {
    defaultTheme: 'custom',
    themes: {
      custom: {
        colors: {
          'green-primary': '#1f5405',
          'green-dark': '#0A2601',
          'green-light': '#E5EDD8',
          'green-light-md': '#99BA73',
          'green-dark-md': '#144003',

          success: '#76ce4aff',
          'success-secondary': '#5be956ff',
          error: '#e63737',
          alert: '#faee6e',

          'neutral-light': '#FCFCFC',
          'neutral-dark': '#040712',

          // 🔴 Vermelhos
          'red-primary': '#b71c1c',
          'red-dark': '#7f0000',
          'red-light': '#ffcdd2',
          'red-light-md': '#ef9a9a',
          'red-dark-md': '#c62828', 

          // 🔵 Azuis
          'blue-primary': '#1565c0',
          'blue-dark': '#0d47a1',
          'blue-light': '#bbdefb',
          'blue-light-md': '#64b5f6',
          'blue-dark-md': '#1976d2'
        },
      },
    },
  },
})
