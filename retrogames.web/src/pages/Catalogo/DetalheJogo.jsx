import { useEffect, useState } from 'react'
import { Link, useLocation, useParams } from 'react-router-dom'
import { obterMensagemErro } from '../../api/erros.js'
import { jogoApi } from '../../api/servicos.js'
import { Carregando, MensagemErro } from '../../components/MensagensEstado.jsx'
import { formatarPreco } from '../../lib/formato.js'

export default function DetalheJogo() {
  const { id } = useParams()
  const { state } = useLocation()
  const [tentativa, setTentativa] = useState(0)
  const [resultado, setResultado] = useState(null)

  const consulta = resultado?.id === id && resultado.tentativa === tentativa ? resultado : null
  const jogo = consulta?.jogo
  const buscaCatalogo = state?.buscaCatalogo ?? ''

  useEffect(() => {
    const controller = new AbortController()

    jogoApi.buscarPorId(id, { signal: controller.signal })
      .then(dados => {
        if (!controller.signal.aborted) setResultado({ id, tentativa, jogo: dados })
      })
      .catch(erro => {
        if (!controller.signal.aborted) {
          setResultado({
            id,
            tentativa,
            erro: obterMensagemErro(erro),
            naoEncontrado: erro?.response?.status === 404,
          })
        }
      })

    return () => controller.abort()
  }, [id, tentativa])

  return (
    <>
      <Link to={{ pathname: '/', search: buscaCatalogo }} className="d-inline-block mb-3">
        Voltar ao catálogo
      </Link>

      {!jogo && <h1 className="mb-3">Detalhe do jogo</h1>}
      {!consulta && <Carregando mensagem="Carregando detalhes…" />}
      {consulta?.erro && (
        <MensagemErro
          mensagem={consulta.erro}
          aoTentarNovamente={consulta.naoEncontrado ? undefined : () => setTentativa(atual => atual + 1)}
        />
      )}

      {jogo && (
        <article className="card border-0 border-top border-primary border-3 shadow-sm">
          <div className="card-body p-4 p-lg-5">
            <div className="row g-4">
              <div className="col-md-8">
                <p className="text-primary fw-semibold mb-2">{jogo.nomePlataforma}</p>
                <h1 className="fw-bold mb-3">{jogo.nome}</h1>
                <ul className="list-unstyled d-flex flex-wrap gap-2 mb-4" aria-label="Gêneros">
                  {jogo.generos.map(genero => (
                    <li key={genero.id} className="badge text-bg-light border fw-normal">{genero.nome}</li>
                  ))}
                </ul>

                <dl className="row mb-0">
                  <dt className="col-sm-5">Plataforma</dt>
                  <dd className="col-sm-7">{jogo.nomePlataforma}</dd>
                  <dt className="col-sm-5">Ano de lançamento</dt>
                  <dd className="col-sm-7">{jogo.anoLancamento}</dd>
                  <dt className="col-sm-5">Condição</dt>
                  <dd className="col-sm-7">{jogo.descricaoCondicaoJogo}</dd>
                </dl>

                {jogo.descricao?.trim() && (
                  <section className="mt-3" aria-labelledby="descricao-jogo">
                    <h2 className="h5" id="descricao-jogo">Descrição</h2>
                    <p className="mb-0">{jogo.descricao}</p>
                  </section>
                )}
              </div>

              <div className="col-md-4">
                <div className="border rounded-3 bg-body-tertiary p-3">
                  <p className="small text-body-secondary mb-1">Preço</p>
                  <p className="fs-3 fw-bold mb-3">{formatarPreco(jogo.preco)}</p>
                  <p className="small text-body-secondary mb-1">Estoque</p>
                  <p className="mb-0">
                    {jogo.estoque === 0
                      ? <span className="badge text-bg-secondary">Esgotado</span>
                      : `${jogo.estoque} ${jogo.estoque === 1 ? 'unidade disponível' : 'unidades disponíveis'}`}
                  </p>
                </div>
              </div>
            </div>
          </div>
        </article>
      )}
    </>
  )
}
