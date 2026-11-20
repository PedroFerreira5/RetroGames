import { useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { obterMensagemErro } from '../../api/erros.js'
import { generoApi, jogoApi, plataformaApi } from '../../api/servicos.js'
import CardJogo from '../../components/CardJogo.jsx'
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado.jsx'
import Paginacao from '../../components/Paginacao.jsx'

function lerInteiroPositivo(valor) {
  if (!/^\d+$/.test(valor ?? '')) return undefined

  const numero = Number(valor)
  return numero > 0 && numero <= 2147483647 ? numero : undefined
}

export default function Catalogo() {
  const [parametros, setParametros] = useSearchParams()
  const busca = parametros.get('busca') ?? ''
  const idPlataforma = lerInteiroPositivo(parametros.get('idPlataforma'))
  const idGenero = lerInteiroPositivo(parametros.get('idGenero'))
  const pagina = lerInteiroPositivo(parametros.get('pagina')) ?? 1

  const [tentativaFiltros, setTentativaFiltros] = useState(0)
  const [tentativaJogos, setTentativaJogos] = useState(0)
  const [filtros, setFiltros] = useState(null)
  const [resultado, setResultado] = useState(null)
  const chave = JSON.stringify([busca, idPlataforma, idGenero, pagina, tentativaJogos])
  const consulta = resultado?.chave === chave ? resultado : null
  const erroFiltros = filtros?.tentativa === tentativaFiltros ? filtros.erro : null

  useEffect(() => {
    const controller = new AbortController()
    const opcoes = { signal: controller.signal }

    Promise.all([plataformaApi.listar(opcoes), generoApi.listar(opcoes)])
      .then(([plataformas, generos]) => {
        if (!controller.signal.aborted) {
          setFiltros({ tentativa: tentativaFiltros, plataformas, generos })
        }
      })
      .catch(erro => {
        if (!controller.signal.aborted) {
          setFiltros({ tentativa: tentativaFiltros, erro: obterMensagemErro(erro) })
        }
      })

    return () => controller.abort()
  }, [tentativaFiltros])

  useEffect(() => {
    const controller = new AbortController()
    const filtrosJogos = { busca: busca || undefined, idPlataforma, idGenero, pagina, tamanhoPagina: 12 }

    jogoApi.listar(filtrosJogos, { signal: controller.signal })
      .then(dados => {
        if (!controller.signal.aborted) setResultado({ chave, dados })
      })
      .catch(erro => {
        if (!controller.signal.aborted) setResultado({ chave, erro: obterMensagemErro(erro) })
      })

    return () => controller.abort()
  }, [busca, idPlataforma, idGenero, pagina, chave])

  function atualizarParametro(nome, valor) {
    const proximos = new URLSearchParams(parametros)

    if (valor) proximos.set(nome, valor)
    else proximos.delete(nome)

    if (nome !== 'pagina') proximos.delete('pagina')
    setParametros(proximos)
  }

  function buscar(evento) {
    evento.preventDefault()
    const valor = new FormData(evento.currentTarget).get('busca').trim()
    atualizarParametro('busca', valor)
  }

  function tentarJogosNovamente() {
    if (erroFiltros) setTentativaFiltros(atual => atual + 1)
    setTentativaJogos(atual => atual + 1)
  }

  return (
    <>
      <h1 className="fw-bold mb-4">Catálogo de jogos</h1>

      <div className="bg-body border rounded-3 p-3 p-md-4 mb-4">
        <div className="row g-3">
          <form className="col-lg-6" onSubmit={buscar}>
            <label className="form-label fw-semibold" htmlFor="busca">Buscar jogo</label>
            <div className="input-group">
              <input
                className="form-control"
                type="search"
                id="busca"
                name="busca"
                key={busca}
                defaultValue={busca}
                placeholder="Nome do jogo"
              />
              <button className="btn btn-primary" type="submit">Buscar</button>
            </div>
          </form>
          <div className="col-sm-6 col-lg-3">
            <label className="form-label fw-semibold" htmlFor="plataforma">Plataforma</label>
            <select
              id="plataforma"
              className="form-select"
              value={idPlataforma ?? ''}
              disabled={!filtros?.plataformas}
              onChange={evento => atualizarParametro('idPlataforma', evento.target.value)}
            >
              <option value="">Todas as plataformas</option>
              {filtros?.plataformas?.map(item => <option key={item.id} value={item.id}>{item.nome}</option>)}
            </select>
          </div>
          <div className="col-sm-6 col-lg-3">
            <label className="form-label fw-semibold" htmlFor="genero">Gênero</label>
            <select
              id="genero"
              className="form-select"
              value={idGenero ?? ''}
              disabled={!filtros?.generos}
              onChange={evento => atualizarParametro('idGenero', evento.target.value)}
            >
              <option value="">Todos os gêneros</option>
              {filtros?.generos?.map(item => <option key={item.id} value={item.id}>{item.nome}</option>)}
            </select>
          </div>
        </div>
      </div>

      {erroFiltros && consulta?.dados && (
        <MensagemErro mensagem={erroFiltros} aoTentarNovamente={() => setTentativaFiltros(atual => atual + 1)} />
      )}
      {!consulta && <Carregando mensagem="Carregando jogos…" />}
      {consulta?.erro && <MensagemErro mensagem={consulta.erro} aoTentarNovamente={tentarJogosNovamente} />}
      {consulta?.dados && (
        <>
          {consulta.dados.itens.length === 0 ? (
            <EstadoVazio mensagem="Nenhum jogo encontrado." />
          ) : (
            <div className="row row-cols-1 row-cols-sm-2 row-cols-lg-3 row-cols-xl-4 g-3">
              {consulta.dados.itens.map(jogo => (
                <div className="col" key={jogo.id}><CardJogo jogo={jogo} buscaCatalogo={parametros.toString()} /></div>
              ))}
            </div>
          )}
          <Paginacao
            pagina={consulta.dados.pagina}
            totalPaginas={consulta.dados.totalPaginas}
            aoMudarPagina={numero => atualizarParametro('pagina', numero === 1 ? '' : numero)}
          />
        </>
      )}
    </>
  )
}
