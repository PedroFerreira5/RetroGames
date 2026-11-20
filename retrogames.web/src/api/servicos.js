import { api } from './api.js'

async function obterDados(rota, opcoes) {
  const resposta = await api.get(rota, opcoes)
  return resposta.data.data
}

export const jogoApi = {
  listar(filtros, { signal } = {}) {
    return obterDados('/Jogo', { params: filtros, signal })
  },

  buscarPorId(id, { signal } = {}) {
    return obterDados(`/Jogo/${encodeURIComponent(id)}`, { signal })
  },
}

export const plataformaApi = {
  listar({ signal } = {}) {
    return obterDados('/Plataforma', { signal })
  },
}

export const generoApi = {
  listar({ signal } = {}) {
    return obterDados('/Genero', { signal })
  },
}

export const condicaoJogoApi = {
  listar({ signal } = {}) {
    return obterDados('/CondicaoJogo', { signal })
  },
}