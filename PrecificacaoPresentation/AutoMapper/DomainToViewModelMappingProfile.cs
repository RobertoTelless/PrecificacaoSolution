using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using EntitiesServices.Model;
using ERP_Condominios_Solution.ViewModels;

namespace MvcMapping.Mappers
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<USUARIO, UsuarioViewModel>();
            CreateMap<USUARIO, UsuarioLoginViewModel>();
            CreateMap<LOG, LogViewModel>();
            CreateMap<CONFIGURACAO, ConfiguracaoViewModel>();
            CreateMap<CARGO_USUARIO, CargoViewModel>();
            CreateMap<NOTICIA, NoticiaViewModel>();
            CreateMap<NOTICIA_COMENTARIO, NoticiaComentarioViewModel>();
            CreateMap<NOTIFICACAO, NotificacaoViewModel>();
            CreateMap<TEMPLATE, TemplateViewModel>();
            CreateMap<TAREFA, TarefaViewModel>();
            CreateMap<CATEGORIA_AGENDA, CategoriaAgendaViewModel>();
            CreateMap<AGENDA, AgendaViewModel>();
            CreateMap<TAREFA_ACOMPANHAMENTO, TarefaAcompanhamentoViewModel>();
            CreateMap<CATEGORIA_NOTIFICACAO, CategoriaNotificacaoViewModel>();
            CreateMap<CATEGORIA_USUARIO, CategoriaUsuarioViewModel>();
            CreateMap<EMPRESA, EmpresaViewModel>();
            CreateMap<FORMA_PAGTO_RECTO, FormaPagRecViewModel>();
            CreateMap<MAQUINA, MaquinaViewModel>();
            CreateMap<PESSOA_EXTERNA, PessoaExternaViewModel>();
            CreateMap<PESSOA_EXTERNA_ANOTACAO, PessoaExternaAnotacaoViewModel>();
            CreateMap<PLATAFORMA_ENTREGA, PlataformaEntregaViewModel>();
            CreateMap<USUARIO_ANOTACAO, UsuarioAnotacaoViewModel>();

        }
    }
}
