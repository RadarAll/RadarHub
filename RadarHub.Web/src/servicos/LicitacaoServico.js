import axios from 'axios'

const API_PREFIX = '/api/Licitacao'

export default {
  async obterOData(parametros = '') {
    try {
      const url = `${API_PREFIX}/OData?${parametros}`
      const { data } = await axios.get(url)
      return data
    } catch (error) {
      throw new Error('Erro ao obter licitações: ' + (error.response?.data?.message || error.message))
    }
  },

  async importar() {
    try {
      const { data } = await axios.post(`${API_PREFIX}/Importar`)
      return data
    } catch (error) {
      throw new Error('Erro na importação: ' + (error.response?.data?.message || error.message))
    }
  }
}
