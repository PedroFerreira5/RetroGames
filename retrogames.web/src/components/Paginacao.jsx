export default function Paginacao({ pagina, totalPaginas, aoMudarPagina }) {
  if (totalPaginas <= 1) return null

  const paginas = [...new Set([1, pagina - 1, pagina, pagina + 1, totalPaginas])]
    .filter(numero => numero >= 1 && numero <= totalPaginas)
    .sort((a, b) => a - b)

  return (
    <nav aria-label="Paginação do catálogo" className="mt-4">
      <ul className="pagination flex-wrap mb-0">
        <li className="page-item">
          <button
            type="button"
            className="page-link"
            disabled={pagina <= 1}
            onClick={() => aoMudarPagina(Math.min(pagina - 1, totalPaginas))}
          >
            Anterior
          </button>
        </li>
        {paginas.map((numero, indice) => (
          <li key={numero} className="page-item d-flex">
            {numero - paginas[indice - 1] > 1 && <span className="page-link text-body-secondary">…</span>}
            <button
              type="button"
              className={`page-link${numero === pagina ? ' active' : ''}`}
              aria-current={numero === pagina ? 'page' : undefined}
              aria-label={`Página ${numero}`}
              onClick={() => aoMudarPagina(numero)}
            >
              {numero}
            </button>
          </li>
        ))}
        <li className="page-item">
          <button
            type="button"
            className="page-link"
            disabled={pagina >= totalPaginas}
            onClick={() => aoMudarPagina(pagina + 1)}
          >
            Próxima
          </button>
        </li>
      </ul>
    </nav>
  )
}
