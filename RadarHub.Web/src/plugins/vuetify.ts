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
          'red-primary': '#b71c1c',     // vermelho principal
          'red-dark': '#7f0000',        // vermelho bem escuro
          'red-light': '#ffcdd2',       // vermelho claro
          'red-light-md': '#ef9a9a',    // vermelho médio claro
          'red-dark-md': '#c62828',     // vermelho médio escuro

          // 🔵 Azuis
          'blue-primary': '#1565c0',    // azul principal
          'blue-dark': '#0d47a1',       // azul bem escuro
          'blue-light': '#bbdefb',      // azul claro
          'blue-light-md': '#64b5f6',   // azul médio claro
          'blue-dark-md': '#1976d2'     // azul médio escuro
        },
      },
    },
  },
})
