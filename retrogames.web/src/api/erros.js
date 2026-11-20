export function obterMensagemErro(erro) {
  const mensagem = erro?.response?.data?.errorMessage

  if (typeof mensagem === 'string' && mensagem.trim()) {
    return mensagem
  }

  return 'Não foi possível conectar ao servidor.'
}