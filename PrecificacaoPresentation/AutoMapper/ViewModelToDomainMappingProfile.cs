using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using EntitiesServices.Model;
using ERP_Condominios_Solution.ViewModels;

namespace MvcMapping.Mappers
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<UsuarioViewModel, USUARIO>();
            CreateMap<UsuarioLoginViewModel, USUARIO>();
            CreateMap<LogViewModel, LOG>();
            CreateMap<ConfiguracaoViewModel, CONFIGURACAO>();
            CreateMap<CargoViewModel, CARGO_USUARIO>();
            CreateMap<NoticiaViewModel, NOTICIA>();
            CreateMap<NoticiaComentarioViewModel, NOTICIA_COMENTARIO>();
            CreateMap<NotificacaoViewModel, NOTIFICACAO>();
            CreateMap<TemplateViewModel, TEMPLATE>();
            CreateMap<TarefaViewModel, TAREFA>();
            CreateMap<CategoriaAgendaViewModel, CATEGORIA_AGENDA>();
            CreateMap<AgendaViewModel, AGENDA>();
            CreateMap<TarefaAcompanhamentoViewModel, TAREFA_ACOMPANHAMENTO>();
            CreateMap<CategoriaNotificacaoViewModel, CATEGORIA_NOTIFICACAO>();
            CreateMap<CategoriaUsuarioViewModel, CATEGORIA_USUARIO>();
            CreateMap<EmpresaViewModel, EMPRESA>();
            CreateMap<FormaPagRecViewModel, FORMA_PAGTO_RECTO>();
            CreateMap<MaquinaViewModel, MAQUINA>();
            CreateMap<PessoaExternaViewModel, PESSOA_EXTERNA>();
            CreateMap<PessoaExternaAnotacaoViewModel, PESSOA_EXTERNA_ANOTACAO>();
            CreateMap<PlataformaEntregaViewModel, PLATAFORMA_ENTREGA>();
            CreateMap<UsuarioAnotacaoViewModel, USUARIO_ANOTACAO>();

        }
    }
}