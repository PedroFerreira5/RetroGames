import { Link } from 'react-router-dom'

export default function NaoEncontrada() {
  return (
    <>
      <h1>Página não encontrada</h1>
      <Link to="/" className="btn btn-primary mt-2">Voltar ao catálogo</Link>
    </>
  )
}
