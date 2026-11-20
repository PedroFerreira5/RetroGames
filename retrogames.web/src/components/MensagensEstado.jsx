export function Carregando({ mensagem = 'Carregando…' }) {
  return (
    <div className="d-flex align-items-center gap-2 py-3" role="status">
      <span className="spinner-border spinner-border-sm" aria-hidden="true" />
      <span>{mensagem}</span>
    </div>
  )
}

export function MensagemErro({ mensagem, aoTentarNovamente }) {
  return (
    <div className="alert alert-danger" role="alert">
      <p className={aoTentarNovamente ? 'mb-2' : 'mb-0'}>{mensagem}</p>
      {aoTentarNovamente && (
        <button type="button" className="btn btn-outline-danger btn-sm" onClick={aoTentarNovamente}>
          Tentar novamente
        </button>
      )}
    </div>
  )
}

export function EstadoVazio({ mensagem = 'Nenhum resultado encontrado.' }) {
  return <p className="border rounded bg-body p-3 mb-0">{mensagem}</p>
}
