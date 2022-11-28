using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using Ninject.Web.Common;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using ModelServices.Interfaces.Repositories;
using ApplicationServices.Services;
using ModelServices.EntitiesServices;
using DataServices.Repositories;
using Ninject.Web.Common.WebHost;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Presentation.Start.NinjectWebCommons), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Presentation.Start.NinjectWebCommons), "Stop")]

namespace Presentation.Start
{
    public class NinjectWebCommons
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind(typeof(IAppServiceBase<>)).To(typeof(AppServiceBase<>));
            kernel.Bind<IUsuarioAppService>().To<UsuarioAppService>();
            kernel.Bind<ILogAppService>().To<LogAppService>();
            kernel.Bind<IConfiguracaoAppService>().To<ConfiguracaoAppService>();
            kernel.Bind<INoticiaAppService>().To<NoticiaAppService>();
            kernel.Bind<INotificacaoAppService>().To<NotificacaoAppService>();
            kernel.Bind<ITemplateAppService>().To<TemplateAppService>();
            kernel.Bind<ITarefaAppService>().To<TarefaAppService>();
            kernel.Bind<IAgendaAppService>().To<AgendaAppService>();
            kernel.Bind<IAssinanteAppService>().To<AssinanteAppService>();
            kernel.Bind<ICategoriaAgendaAppService>().To<CategoriaAgendaAppService>();
            kernel.Bind<ICategoriaNotificacaoAppService>().To<CategoriaNotificacaoAppService>();
            kernel.Bind<ICategoriaUsuarioAppService>().To<CategoriaUsuarioAppService>();
            kernel.Bind<IEmpresaAppService>().To<EmpresaAppService>();
            kernel.Bind<IFormaPagRecAppService>().To<FormaPagRecAppService>();
            kernel.Bind<IMaquinaAppService>().To<MaquinaAppService>();
            kernel.Bind<IPeriodicidadeAppService>().To<PeriodicidadeAppService>();
            kernel.Bind<IPessoaExternaAppService>().To<PessoaExternaAppService>();
            kernel.Bind<IPlataformaEntregaAppService>().To<PlataformaEntregaAppService>();
            kernel.Bind<ICargoAppService>().To<CargoAppService>();
            kernel.Bind<ICentroCustoAppService>().To<CentroCustoAppService>();
            kernel.Bind<IGrupoCCAppService>().To<GrupoCCAppService>();
            kernel.Bind<ISubgrupoAppService>().To<SubgrupoAppService>();
            kernel.Bind<IBancoAppService>().To<BancoAppService>();
            kernel.Bind<IContaBancariaAppService>().To<ContaBancariaAppService>();
            kernel.Bind<IVideoAppService>().To<VideoAppService>();
            kernel.Bind<ICategoriaFornecedorAppService>().To<CategoriaFornecedorAppService>();
            kernel.Bind<IFornecedorAppService>().To<FornecedorAppService>();
            kernel.Bind<IFornecedorCnpjAppService>().To<FornecedorCnpjAppService>();
            kernel.Bind<ICategoriaClienteAppService>().To<CategoriaClienteAppService>();
            kernel.Bind<IClienteAppService>().To<ClienteAppService>();
            kernel.Bind<IClienteCnpjAppService>().To<ClienteCnpjAppService>();
            kernel.Bind<ICategoriaProdutoAppService>().To<CategoriaProdutoAppService>();
            kernel.Bind<ISubcategoriaProdutoAppService>().To<SubcategoriaProdutoAppService>();
            kernel.Bind<IUnidadeAppService>().To<UnidadeAppService>();
            kernel.Bind<ITamanhoAppService>().To<TamanhoAppService>();
            kernel.Bind<IProdutoAppService>().To<ProdutoAppService>();
            kernel.Bind<IProdutoEstoqueFilialAppService>().To<ProdutoEstoqueFilialAppService>();
            kernel.Bind<IProdutotabelaPrecoAppService>().To<ProdutoTabelaPrecoAppService>();
            kernel.Bind<IMovimentoEstoqueProdutoAppService>().To<MovimentoEstoqueProdutoAppService>();
            kernel.Bind<ITipoEmbalagemAppService>().To<TipoEmbalagemAppService>();
            kernel.Bind<IFichaTecnicaAppService>().To<FichaTecnicaAppService>();
            kernel.Bind<ITipoAcaoAppService>().To<TipoAcaoAppService>();
            kernel.Bind<IMotivoCancelamentoAppService>().To<MotivoCancelamentoAppService>();
            kernel.Bind<IMotivoEncerramentoAppService>().To<MotivoEncerramentoAppService>();
            kernel.Bind<IContaPagarAppService>().To<ContaPagarAppService>();
            kernel.Bind<IContaPagarParcelaAppService>().To<ContaPagarParcelaAppService>();
            kernel.Bind<IContaPagarRateioAppService>().To<ContaPagarRateioAppService>();
            kernel.Bind<IContaReceberAppService>().To<ContaReceberAppService>();
            kernel.Bind<IContaReceberParcelaAppService>().To<ContaReceberParcelaAppService>();
            kernel.Bind<IContaReceberRateioAppService>().To<ContaReceberRateioAppService>();
            kernel.Bind<ICRMAppService>().To<CRMAppService>();
            kernel.Bind<ICRMOrigemAppService>().To<CRMOrigemAppService>();
            kernel.Bind<ICategoriaServicoAppService>().To<CategoriaServicoAppService>();
            kernel.Bind<IServicoAppService>().To<ServicoAppService>();
            kernel.Bind<ITransportadoraAppService>().To<TransportadoraAppService>();
            kernel.Bind<ITemplateEMailAppService>().To<TemplateEMailAppService>();
            kernel.Bind<ITemplateSMSAppService>().To<TemplateSMSAppService>();
            kernel.Bind<ITemplatePropostaAppService>().To<TemplatePropostaAppService>();
            kernel.Bind<IFormaEnvioAppService>().To<FormaEnvioAppService>();
            kernel.Bind<IFormaFreteAppService>().To<FormaFreteAppService>();
            kernel.Bind<IPedidoCompraAppService>().To<PedidoCompraAppService>();
            kernel.Bind<IAssinanteCnpjAppService>().To<AssinanteCnpjAppService>();
            kernel.Bind<IPlanoAppService>().To<PlanoAppService>();
            kernel.Bind<ICRMDiarioAppService>().To<CRMDiarioAppService>();

            kernel.Bind(typeof(IServiceBase<>)).To(typeof(ServiceBase<>));
            kernel.Bind<IUsuarioService>().To<UsuarioService>();
            kernel.Bind<ILogService>().To<LogService>();
            kernel.Bind<IPerfilService>().To<PerfilService>();
            kernel.Bind<IConfiguracaoService>().To<ConfiguracaoService>();
            kernel.Bind<INotificacaoService>().To<NotificacaoService>();
            kernel.Bind<INoticiaService>().To<NoticiaService>();
            kernel.Bind<ITipoPessoaService>().To<TipoPessoaService>();
            kernel.Bind<ITemplateService>().To<TemplateService>();
            kernel.Bind<ITarefaService>().To<TarefaService>();
            kernel.Bind<IAgendaService>().To<AgendaService>();
            kernel.Bind<IAssinanteService>().To<AssinanteService>();
            kernel.Bind<ICategoriaAgendaService>().To<CategoriaAgendaService>();
            kernel.Bind<ICategoriaNotificacaoService>().To<CategoriaNotificacaoService>();
            kernel.Bind<ICategoriaUsuarioService>().To<CategoriaUsuarioService>();
            kernel.Bind<IEmpresaService>().To<EmpresaService>();
            kernel.Bind<IFormaPagRecService>().To<FormaPagRecService>();
            kernel.Bind<IMaquinaService>().To<MaquinaService>();
            kernel.Bind<IPeriodicidadeService>().To<PeriodicidadeService>();
            kernel.Bind<IPessoaExternaService>().To<PessoaExternaService>();
            kernel.Bind<IPlataformaEntregaService>().To<PlataformaEntregaService>();
            kernel.Bind<ICargoService>().To<CargoService>();
            kernel.Bind<ICentroCustoService>().To<CentroCustoService>();
            kernel.Bind<IGrupoCCService>().To<GrupoCCService>();
            kernel.Bind<ISubgrupoService>().To<SubgrupoService>();
            kernel.Bind<IBancoService>().To<BancoService>();
            kernel.Bind<IContaBancariaService>().To<ContaBancariaService>();
            kernel.Bind<IVideoService>().To<VideoService>();
            kernel.Bind<ICategoriaFornecedorService>().To<CategoriaFornecedorService>();
            kernel.Bind<IFornecedorService>().To<FornecedorService>();
            kernel.Bind<IFornecedorCnpjService>().To<FornecedorCnpjService>();
            kernel.Bind<ICategoriaClienteService>().To<CategoriaClienteService>();
            kernel.Bind<IClienteService>().To<ClienteService>();
            kernel.Bind<IClienteCnpjService>().To<ClienteCnpjService>();
            kernel.Bind<ICategoriaProdutoService>().To<CategoriaProdutoService>();
            kernel.Bind<ISubcategoriaProdutoService>().To<SubcategoriaProdutoService>();
            kernel.Bind<IUnidadeService>().To<UnidadeService>();
            kernel.Bind<ITamanhoService>().To<TamanhoService>();
            kernel.Bind<IProdutoService>().To<ProdutoService>();
            kernel.Bind<IProdutoEstoqueFilialService>().To<ProdutoEstoqueFilialService>();
            kernel.Bind<IProdutoMovimentoEstoqueService>().To<ProdutoMovimentoEstoqueService>();
            kernel.Bind<IProdutoTabelaPrecoService>().To<ProdutoTabelaPrecoService>();
            kernel.Bind<IMovimentoEstoqueProdutoService>().To<MovimentoEstoqueProdutoService>();
            kernel.Bind<ITipoEmbalagemService>().To<TipoEmbalagemService>();
            kernel.Bind<IFichaTecnicaService>().To<FichaTecnicaService>();
            kernel.Bind<IMotivoCancelamentoService>().To<MotivoCancelamentoService>();
            kernel.Bind<IMotivoEncerramentoService>().To<MotivoEncerramentoService>();
            kernel.Bind<IContaPagarService>().To<ContaPagarService>();
            kernel.Bind<IContaPagarParcelaService>().To<ContaPagarParcelaService>();
            kernel.Bind<IContaPagarRateioService>().To<ContaPagarRateioService>();
            kernel.Bind<IContaReceberService>().To<ContaReceberService>();
            kernel.Bind<IContaReceberParcelaService>().To<ContaReceberParcelaService>();
            kernel.Bind<IContaReceberRateioService>().To<ContaReceberRateioService>();
            kernel.Bind<ICRMService>().To<CRMService>();
            kernel.Bind<ICRMOrigemService>().To<CRMOrigemService>();
            kernel.Bind<ICategoriaServicoService>().To<CategoriaServicoService>();
            kernel.Bind<IServicoService>().To<ServicoService>();
            kernel.Bind<ITransportadoraService>().To<TransportadoraService>();
            kernel.Bind<ITemplateEMailService>().To<TemplateEMailService>();
            kernel.Bind<ITemplateSMSService>().To<TemplateSMSService>();
            kernel.Bind<ITipoAcaoService>().To<TipoAcaoService>();
            kernel.Bind<ITemplatePropostaService>().To<TemplatePropostaService>();
            kernel.Bind<IFormaEnvioService>().To<FormaEnvioService>();
            kernel.Bind<IFormaFreteService>().To<FormaFreteService>();
            kernel.Bind<IPedidoCompraService>().To<PedidoCompraService>();
            kernel.Bind<IPlanoService>().To<PlanoService>();
            kernel.Bind<IAssinanteCnpjService>().To<AssinanteCnpjService>();
            kernel.Bind<ICRMDiarioService>().To<CRMDiarioService>();

            kernel.Bind(typeof(IRepositoryBase<>)).To(typeof(RepositoryBase<>));
            kernel.Bind<IConfiguracaoRepository>().To<ConfiguracaoRepository>();
            kernel.Bind<IUsuarioRepository>().To<UsuarioRepository>();
            kernel.Bind<ILogRepository>().To<LogRepository>();
            kernel.Bind<IPerfilRepository>().To<PerfilRepository>();
            kernel.Bind<ITemplateRepository>().To<TemplateRepository>();
            kernel.Bind<ITipoPessoaRepository>().To<TipoPessoaRepository>();
            kernel.Bind<ICategoriaNotificacaoRepository>().To<CategoriaNotificacaoRepository>();
            kernel.Bind<INotificacaoRepository>().To<NotificacaoRepository>();
            kernel.Bind<INoticiaRepository>().To<NoticiaRepository>();
            kernel.Bind<INoticiaComentarioRepository>().To<NoticiaComentarioRepository>();
            kernel.Bind<INotificacaoAnexoRepository>().To<NotificacaoAnexoRepository>();
            kernel.Bind<ITarefaRepository>().To<TarefaRepository>();
            kernel.Bind<ITipoTarefaRepository>().To<TipoTarefaRepository>();
            kernel.Bind<ITarefaAnexoRepository>().To<TarefaAnexoRepository>();
            kernel.Bind<IUsuarioAnexoRepository>().To<UsuarioAnexoRepository>();
            kernel.Bind<IUFRepository>().To<UFRepository>();
            kernel.Bind<IAgendaRepository>().To<AgendaRepository>();
            kernel.Bind<IAgendaAnexoRepository>().To<AgendaAnexoRepository>();
            kernel.Bind<ICategoriaAgendaRepository>().To<CategoriaAgendaRepository>();
            kernel.Bind<IAssinanteRepository>().To<AssinanteRepository>();
            kernel.Bind<IPeriodicidadeRepository>().To<PeriodicidadeRepository>();
            kernel.Bind<ICargoRepository>().To<CargoRepository>();
            kernel.Bind<ICategoriaUsuarioRepository>().To<CategoriaUsuarioRepository>();
            kernel.Bind<IEmpresaRepository>().To<EmpresaRepository>();
            kernel.Bind<IFormaPagRecRepository>().To<FormaPagRecRepository>();
            kernel.Bind<IMaquinaRepository>().To<MaquinaRepository>();
            kernel.Bind<IPessoaExternaRepository>().To<PessoaExternaRepository>();
            kernel.Bind<IPessoaExternaAnexoRepository>().To<PessoaExternaAnexoRepository>();
            kernel.Bind<IPessoaExternaAnotacaoRepository>().To<PessoaExternaAnotacaoRepository>();
            kernel.Bind<IPlataformaEntregaRepository>().To<PlataformaEntregaRepository>();
            kernel.Bind<IUsuarioAnotacaoRepository>().To<UsuarioAnotacaoRepository>();
            kernel.Bind<IRegimeTributarioRepository>().To<RegimeTributarioRepository>();
            kernel.Bind<IEmpresaAnexoRepository>().To<EmpresaAnexoRepository>();
            kernel.Bind<ICentroCustoRepository>().To<CentroCustoRepository>();
            kernel.Bind<IGrupoCCRepository>().To<GrupoCCRepository>();
            kernel.Bind<ISubgrupoRepository>().To<SubgrupoRepository>();
            kernel.Bind<IBancoRepository>().To<BancoRepository>();
            kernel.Bind<IContaBancariaRepository>().To<ContaBancariaRepository>();
            kernel.Bind<IContaBancariaContatoRepository>().To<ContaBancariaContatoRepository>();
            kernel.Bind<IContaBancariaLancamentoRepository>().To<ContaBancariaLancamentoRepository>();
            kernel.Bind<ITipoContaRepository>().To<TipoContaRepository>();
            kernel.Bind<IVideoRepository>().To<VideoRepository>();
            kernel.Bind<IVideoComentarioRepository>().To<VideoComentarioRepository>();
            kernel.Bind<ICategoriaFornecedorRepository>().To<CategoriaFornecedorRepository>();
            kernel.Bind<ISexoRepository>().To<SexoRepository>();
            kernel.Bind<ITipoContribuinteRepository>().To<TipoContribuinteRepository>();
            kernel.Bind<IFornecedorRepository>().To<FornecedorRepository>();
            kernel.Bind<IFornecedorAnexoRepository>().To<FornecedorAnexoRepository>();
            kernel.Bind<IFornecedorAnotacaoRepository>().To<FornecedorAnotacaoRepository>();
            kernel.Bind<IFornecedorCnpjRepository>().To<FornecedorCnpjRepository>();
            kernel.Bind<IFornecedorContatoRepository>().To<FornecedorContatoRepository>();
            kernel.Bind<ICategoriaClienteRepository>().To<CategoriaClienteRepository>();
            kernel.Bind<IClienteRepository>().To<ClienteRepository>();
            kernel.Bind<IClienteAnexoRepository>().To<ClienteAnexoRepository>();
            kernel.Bind<IClienteAnotacaoRepository>().To<ClienteAnotacaoRepository>();
            kernel.Bind<IClienteContatoRepository>().To<ClienteContatoRepository>();
            kernel.Bind<IClienteReferenciaRepository>().To<ClienteReferenciaRepository>();
            kernel.Bind<IClienteCnpjRepository>().To<ClienteCnpjRepository>();
            kernel.Bind<IEmpresaMaquinaRepository>().To<EmpresaMaquinaRepository>();
            kernel.Bind<ICategoriaProdutoRepository>().To<CategoriaProdutoRepository>();
            kernel.Bind<ISubcategoriaProdutoRepository>().To<SubcategoriaProdutoRepository>();
            kernel.Bind<IUnidadeRepository>().To<UnidadeRepository>();
            kernel.Bind<ITamanhoRepository>().To<TamanhoRepository>();
            kernel.Bind<IProdutoAnexoRepository>().To<ProdutoAnexoRepository>();
            //kernel.Bind<IProdutoBarcodeRepository>().To<ProdutoBarcodeRepository>();
            kernel.Bind<IProdutoEstoqueFilialRepository>().To<ProdutoEstoqueFilialRepository>();
            kernel.Bind<IProdutoFornecedorRepository>().To<ProdutoFornecedorRepository>();
            //kernel.Bind<IProdutoGradeRepository>().To<ProdutoGradeRepository>();
            kernel.Bind<IProdutoMovimentoEstoqueRepository>().To<ProdutoMovimentoEstoqueRepository>();
            kernel.Bind<IProdutoOrigemRepository>().To<ProdutoOrigemRepository>();
            kernel.Bind<IProdutoRepository>().To<ProdutoRepository>();
            kernel.Bind<IProdutoTabelaPrecoRepository>().To<ProdutoTabelaPrecoRepository>();
            kernel.Bind<IMovimentoEstoqueProdutoRepository>().To<MovimentoEstoqueProdutoRepository>();
            kernel.Bind<IProdutoKitRepository>().To<ProdutoKitRepository>();
            kernel.Bind<IFichaTecnicaDetalheRepository>().To<FichaTecnicaDetalheRepository>();
            kernel.Bind<ITipoEmbalagemRepository>().To<TipoEmbalagemRepository>();
            kernel.Bind<IFichaTecnicaRepository>().To<FichaTecnicaRepository>();
            kernel.Bind<ITipoAcaoRepository>().To<TipoAcaoRepository>();
            kernel.Bind<IMotivoCancelamentoRepository>().To<MotivoCancelamentoRepository>();
            kernel.Bind<IMotivoEncerramentoRepository>().To<MotivoEncerramentoRepository>();
            kernel.Bind<IContaPagarRepository>().To<ContaPagarRepository>();
            kernel.Bind<IContaPagarParcelaRepository>().To<ContaPagarParcelaRepository>();
            kernel.Bind<IContaPagarRateioRepository>().To<ContaPagarRateioRepository>();
            kernel.Bind<IContaPagarAnexoRepository>().To<ContaPagarAnexoRepository>();
            kernel.Bind<IContaReceberRepository>().To<ContaReceberRepository>();
            kernel.Bind<IContaReceberParcelaRepository>().To<ContaReceberParcelaRepository>();
            kernel.Bind<IContaReceberRateioRepository>().To<ContaReceberRateioRepository>();
            kernel.Bind<IContaReceberAnexoRepository>().To<ContaReceberAnexoRepository>();
            kernel.Bind<ICRMAcaoRepository>().To<CRMAcaoRepository>();
            kernel.Bind<ICRMAnexoRepository>().To<CRMAnexoRepository>();
            kernel.Bind<ICRMComentarioRepository>().To<CRMComentarioRepository>();
            kernel.Bind<ICRMContatoRepository>().To<CRMContatoRepository>();
            kernel.Bind<ICRMItemPedidoRepository>().To<CRMItemPedidoRepository>();
            kernel.Bind<ICRMOrigemRepository>().To<CRMOrigemRepository>();
            kernel.Bind<ICRMPedidoAnexoRepository>().To<CRMPedidoAnexoRepository>();
            kernel.Bind<ICRMPedidoComentarioRepository>().To<CRMPedidoComentarioRepository>();
            kernel.Bind<ICRMPedidoRepository>().To<CRMPedidoRepository>();
            kernel.Bind<ICRMRepository>().To<CRMRepository>();
            kernel.Bind<ICategoriaServicoRepository>().To<CategoriaServicoRepository>();
            kernel.Bind<IServicoRepository>().To<ServicoRepository>();
            kernel.Bind<ITransportadoraRepository>().To<TransportadoraRepository>();
            kernel.Bind<IFilialRepository>().To<FilialRepository>();
            kernel.Bind<INomencBrasServicosRepository>().To<NomencBrasServicosRepository>();
            kernel.Bind<INomenclaturaRepository>().To<INomenclaturaRepository>();
            kernel.Bind<ITipoVeiculoRepository>().To<TipoVeiculoRepository>();
            kernel.Bind<ITipoTransporteRepository>().To<TipoTransporteRepository>();
            kernel.Bind<ITemplateEMailRepository>().To<TemplateEMailRepository>();
            kernel.Bind<ITemplateSMSRepository>().To<ITemplateSMSRepository>();
            kernel.Bind<ITipoCRMRepository>().To<TipoCRMRepository>();
            kernel.Bind<ITemplatePropostaRepository>().To<TemplatePropostaRepository>();
            kernel.Bind<IFormaEnvioRepository>().To<FormaEnvioRepository>();
            kernel.Bind<IFormaFreteRepository>().To<FormaFreteRepository>();
            kernel.Bind<IPedidoCompraRepository>().To<PedidoCompraRepository>();
            kernel.Bind<IPedidoCompraAnexoRepository>().To<PedidoCompraAnexoRepository>();
            kernel.Bind<IPedidoCompraAcompanhamentoRepository>().To<PedidoCompraAcompanhamentoRepository>();
            kernel.Bind<IItemPedidoCompraRepository>().To<ItemPedidoCompraRepository>();
            kernel.Bind<IAssinanteAnexoRepository>().To<AssinanteAnexoRepository>();
            kernel.Bind<IAssinanteAnotacaoRepository>().To<AssinanteAnotacaoRepository>();
            kernel.Bind<IAssinanteCnpjRepository>().To<AssinanteCnpjRepository>();
            kernel.Bind<IAssinantePagamentoRepository>().To<AssinantePagamentoRepository>();
            kernel.Bind<IAssinantePlanoRepository>().To<AssinantePlanoRepository>();
            kernel.Bind<IPeriodicidadePlanoRepository>().To<PeriodicidadePlanoRepository>();
            kernel.Bind<IPlanoRepository>().To<PlanoRepository>();
            kernel.Bind<ICRMDiarioRepository>().To<CRMDiarioRepository>();


        }
    }
}