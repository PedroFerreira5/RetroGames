import { useState } from 'react'
import { Link, NavLink, Outlet } from 'react-router-dom'

export default function Layout() {
  const [menuAberto, setMenuAberto] = useState(false)

  return (
    <div className="d-flex min-vh-100 flex-column bg-body-tertiary">
      <header className="bg-dark" data-bs-theme="dark">
        <nav className="navbar navbar-expand-md" aria-label="Navegação principal">
          <div className="container">
            <Link className="navbar-brand fw-semibold" to="/" onClick={() => setMenuAberto(false)}>
              RetroGames
            </Link>
            <button
              className="navbar-toggler"
              type="button"
              aria-controls="menu-principal"
              aria-expanded={menuAberto}
              aria-label={menuAberto ? 'Fechar menu' : 'Abrir menu'}
              onClick={() => setMenuAberto(aberto => !aberto)}
            >
              <span className="navbar-toggler-icon" />
            </button>
            <div className={`collapse navbar-collapse${menuAberto ? ' show' : ''}`} id="menu-principal">
              <div className="navbar-nav ms-md-auto">
                <NavLink className="nav-link" to="/" end onClick={() => setMenuAberto(false)}>
                  Catálogo
                </NavLink>
              </div>
            </div>
          </div>
        </nav>
      </header>

      <main className="container flex-grow-1 py-5" id="conteudo">
        <Outlet />
      </main>

      <footer className="border-top bg-body py-3">
        <div className="container small text-body-secondary">RetroGames</div>
      </footer>
    </div>
  )
}
