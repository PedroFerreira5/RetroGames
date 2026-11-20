import { Link } from 'react-router-dom'
import { formatarPreco } from '../lib/formato.js'

export default function CardJogo({ jogo, buscaCatalogo }) {
  return (
    <article className="card h-100 border-0 border-top border-primary border-3 shadow-sm">
      <div className="card-body d-flex flex-column gap-3">
        <div>
          <p className="small fw-semibold text-primary mb-1">{jogo.nomePlataforma}</p>
          <h2 className="h5 mb-1">{jogo.nome}</h2>
          <p className="small text-body-secondary mb-0">
            {jogo.anoLancamento} · {jogo.descricaoCondicaoJogo}
          </p>
        </div>

        <ul className="list-unstyled d-flex flex-wrap gap-1 mb-0" aria-label="Gêneros">
          {jogo.generos.map(genero => (
            <li key={genero} className="badge text-bg-light border fw-normal">{genero}</li>
          ))}
        </ul>

        <div className="mt-auto d-flex flex-wrap align-items-center gap-2">
          <span className="fs-5 fw-bold">{formatarPreco(jogo.preco)}</span>
          {jogo.estoque === 0 && <span className="badge text-bg-secondary">Esgotado</span>}
        </div>

        <Link
          className="btn btn-outline-primary align-self-start"
          to={`/jogos/${jogo.id}`}
          state={{ buscaCatalogo }}
          aria-label={`Ver detalhes de ${jogo.nome}`}
        >
          Ver detalhes
        </Link>
      </div>
    </article>
  )
}
