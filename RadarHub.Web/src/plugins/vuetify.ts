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
          success: '#99f56c',
          error: '#e63737',
          alert: '#faee6e',
          'neutral-light': '#FCFCFC',
          'neutral-dark': '#040712',
        },
      },
    },
  },
})
