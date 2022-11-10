using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Text.RegularExpressions;
using ModelServices.Interfaces.Repositories;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Web.UI.WebControls;
using EntitiesServices.WorkClasses;
using System.Web;

namespace ApplicationServices.Services
{
    public class MensagemAppService : AppServiceBase<MENSAGENS>, IMensagemAppService
    {
        private readonly IMensagemService _baseService;
        private readonly IConfiguracaoService _confService;
        private readonly IMensagemRepository _mensagemRepository;
        private readonly ITemplateEMailAppService _temaApp;
        private readonly IConfiguracaoAppService _confApp;
        


        public MensagemAppService(IMensagemService baseService, IConfiguracaoService confService, IMensagemRepository mensagemRepository, ITemplateEMailAppService temaApp,
            IConfiguracaoAppService confApp) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
            _mensagemRepository = mensagemRepository;
            _temaApp = temaApp;
            _confApp = confApp;            
        }

        public List<MENSAGENS> GetAllItens(Int32 idAss)
        {
            List<MENSAGENS> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<UF> GetAllUF()
        {
            List<UF> lista = _baseService.GetAllUF();
            return lista;
        }

        public UF GetUFbySigla(String sigla)
        {
            return _baseService.GetUFbySigla(sigla);
        }

        public MENSAGEM_ANEXO GetAnexoById(Int32 id)
        {
            return _baseService.GetAnexoById(id);
        }

        public MENSAGENS_DESTINOS GetDestinoById(Int32 id)
        {
            MENSAGENS_DESTINOS lista = _baseService.GetDestinoById(id);
            return lista;
        }

        public List<MENSAGENS> GetAllItensAdm(Int32 idAss)
        {
            List<MENSAGENS> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public MENSAGENS GetItemById(Int32 id)
        {
            MENSAGENS item = _baseService.GetItemById(id);
            return item;
        }

        public MENSAGENS CheckExist(MENSAGENS conta, Int32 idAss)
        {
            MENSAGENS item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<CATEGORIA_CLIENTE> GetAllTipos(Int32 idAss)
        {
            List<CATEGORIA_CLIENTE> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }

        public List<TEMPLATE_SMS> GetAllTemplatesSMS(Int32 idAss)
        {
            List<TEMPLATE_SMS> lista = _baseService.GetAllTemplatesSMS(idAss);
            return lista;
        }

        public Int32 ExecuteFilterSMS(DateTime? envio, Int32 cliente, String texto, Int32 idAss, out List<MENSAGENS> objeto)
        {
            try
            {
                objeto = new List<MENSAGENS>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterSMS(envio, cliente, texto, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ExecuteFilterEMail(DateTime? envio, Int32 cliente, String texto, Int32 idAss, out List<MENSAGENS> objeto)
        {
            try
            {
                objeto = new List<MENSAGENS>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterSMS(envio, cliente, texto, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(MENSAGENS item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.MENS_IN_ATIVO = 1;
                item.MENS_DT_CRIACAO = DateTime.Now;
                item.USUA_CD_ID = usuario.USUA_CD_ID;
                if (item.MENS_NM_LINK != null)
                {
                    if (!item.MENS_NM_LINK.Contains("www."))
                    {
                        item.MENS_NM_LINK = "www." + item.MENS_NM_LINK;
                    }
                    if (!item.MENS_NM_LINK.Contains("http://"))
                    {
                        item.MENS_NM_LINK = "http://" + item.MENS_NM_LINK;
                    }
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddMENS",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<MENSAGENS>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(MENSAGENS item, MENSAGENS itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditMENS",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<MENSAGENS>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<MENSAGENS>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(MENSAGENS item, MENSAGENS itemAntes)
        {
            try
            {
                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(MENSAGENS item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.MENS_IN_ATIVO = 0;
                item.ASSINANTE = null;
                item.USUARIO = null;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelMENS",
                    LOG_TX_REGISTRO = item.MENS_NM_CAMPANHA + "|" + item.MENS_DT_CRIACAO.Value.ToShortDateString() + "|" + item.MENS_IN_TIPO.ToString()
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(MENSAGENS item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.MENS_IN_ATIVO = 1;
                item.ASSINANTE = null;
                item.USUARIO = null;                

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatMENS",
                    LOG_TX_REGISTRO = item.MENS_NM_CAMPANHA + "|" + item.MENS_DT_CRIACAO.Value.ToShortDateString() + "|" + item.MENS_IN_TIPO.ToString()
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditDestino(MENSAGENS_DESTINOS item)
        {
            try
            {
                // Persiste
                return _baseService.EditDestino(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void SendEmailSchedule(int? mensId, HttpServerUtilityBase server)
        {
            
            List<MENSAGENS> mensagens = _mensagemRepository.FilterMensagensNotSend(mensId);

            

            foreach(MENSAGENS mg in mensagens)
            {
                ControlError controlError = new ControlError();

                CONFIGURACAO conf = _confApp.GetItemById(mg.USUARIO.ASSI_CD_ID);

                try
                {
                    List<CLIENTE> listaCli = new List<CLIENTE>();

                    if (mg.GRUPO != null)
                    {
                        foreach (GRUPO_CLIENTE item in mg.GRUPO.GRUPO_CLIENTE)
                        {
                            if (item.GRCL_IN_ATIVO == 1)
                            {
                                listaCli.Add(item.CLIENTE);
                            }
                        }
                    }

                    if (mg.CLIENTE != null)
                    {
                        listaCli.Add(mg.CLIENTE);
                    }

                    // Template HTML
                    if (mg.TEEM_CD_ID != null)
                    {
                        TEMPLATE_EMAIL temp = _temaApp.GetItemById((int)mg.TEEM_CD_ID);
                        if (temp.TEEM_IN_HTML == 1)
                        {
                            // Prepara corpo do e-mail e trata link
                            String corpo = String.Empty;
                            String caminho = "/Imagens/" + mg.ASSI_CD_ID.ToString() + "/TemplatesHTML/" + temp.TEEM_CD_ID.ToString() + "/Arquivos/" + temp.TEEM_AQ_ARQUIVO;
                            String path = Path.Combine(server.MapPath(caminho));
                            using (StreamReader reader = new System.IO.StreamReader(path))
                            {
                                corpo = reader.ReadToEnd();
                            }
                            StringBuilder str = new StringBuilder();
                            str.AppendLine(corpo);
                            String body = str.ToString();
                            String emailBody = body;

                            // Checa e monta anexos
                            List<System.Net.Mail.Attachment> listaAnexo = new List<System.Net.Mail.Attachment>();
                            if (mg.MENSAGEM_ANEXO.Count > 0)
                            {
                                foreach (MENSAGEM_ANEXO item in mg.MENSAGEM_ANEXO)
                                {
                                    String fn = server.MapPath(item.MEAN_AQ_ARQUIVO);
                                    System.Net.Mail.Attachment anexo = new System.Net.Mail.Attachment(fn);
                                    listaAnexo.Add(anexo);
                                }
                            }

                            // Monta mensagem
                            NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                            Email mensagem = new Email();
                            mensagem.ASSUNTO = mg.MENS_NM_CAMPANHA != null ? mg.MENS_NM_CAMPANHA : "Assunto Diverso";
                            mensagem.CORPO = emailBody;
                            mensagem.DEFAULT_CREDENTIALS = false;
                            mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                            mensagem.ENABLE_SSL = true;
                            mensagem.NOME_EMISSOR = mg.USUARIO.ASSINANTE.ASSI_NM_NOME;
                            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                            mensagem.NETWORK_CREDENTIAL = net;
                            mensagem.ATTACHMENT = listaAnexo;

                            // Monta destinos
                            MailAddressCollection col = new MailAddressCollection();
                            foreach (CLIENTE item in listaCli)
                            {
                                col.Add(item.CLIE_NM_EMAIL);
                            }

                            // Envia mensagem
                            try
                            {
                                Int32 voltaMail = CommunicationPackage.SendEmailCollection(mensagem, col);

                                controlError.IsOK = true;
                            }
                            catch (Exception ex)
                            {
                                controlError.IsOK = false;
                                controlError.HandleExeption(ex);

                                if (ex.GetType() == typeof(SmtpFailedRecipientException))
                                {
                                    var se = (SmtpFailedRecipientException)ex;
                                    controlError.HandleExeption(se.FailedRecipient);
                                }
                            }

                            // Grava mensagem/destino e erros
                            if (controlError.IsOK)
                            {
                                foreach (CLIENTE item in listaCli)
                                {
                                    MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                                    dest.MEDE_IN_ATIVO = 1;
                                    dest.MEDE_IN_POSICAO = 1;
                                    dest.MEDE_IN_STATUS = 1;
                                    dest.CLIE_CD_ID = item.CLIE_CD_ID;
                                    dest.MEDE_DS_ERRO_ENVIO = controlError.GetMenssage();
                                    dest.MENS_CD_ID = mg.MENS_CD_ID;
                                    mg.MENSAGENS_DESTINOS.Add(dest);
                                    mg.MENS_DT_ENVIO = DateTime.Now;
                                    mg.MENS_TX_TEXTO = body;
                                    mg.MENS_IN_STATUS = (int)Enumerador.StatusEnvioEmail.ENVIADO;
                                    _mensagemRepository.Update(mg);
                                }
                            }
                            else
                            {
                                mg.MENS_TX_RETORNO = controlError.GetMenssage();
                                mg.MENS_IN_STATUS = (int)Enumerador.StatusEnvioEmail.ERRO;
                                _mensagemRepository.Update(mg);
                            }
                        }
                        else
                        {
                            // Prepara inicial
                            String body = String.Empty;
                            String header = String.Empty;
                            String footer = String.Empty;
                            String link = String.Empty;
                            if (mg.TEEM_CD_ID != null)
                            {
                                body = temp.TEEM_TX_CORPO;
                                header = temp.TEEM_TX_CABECALHO;
                                footer = temp.TEEM_TX_DADOS;
                                link = temp.TEEM_LK_LINK;
                            }

                            // Trata corpo
                            StringBuilder str = new StringBuilder();
                            str.AppendLine(body);

                            // Trata link
                            if (!String.IsNullOrEmpty(link))
                            {
                                if (!link.Contains("www."))
                                {
                                    link = "www." + link;
                                }
                                if (!link.Contains("http://"))
                                {
                                    link = "http://" + link;
                                }
                                str.AppendLine("<a href='" + link + "'>Clique aqui para maiores informações</a>");
                            }
                            body = str.ToString();

                            // Checa e monta anexos
                            List<System.Net.Mail.Attachment> listaAnexo = new List<System.Net.Mail.Attachment>();
                            if (mg.MENSAGEM_ANEXO.Count > 0)
                            {
                                foreach (MENSAGEM_ANEXO item in mg.MENSAGEM_ANEXO)
                                {
                                    String fn = server.MapPath(item.MEAN_AQ_ARQUIVO);
                                    System.Net.Mail.Attachment anexo = new System.Net.Mail.Attachment(fn);
                                    listaAnexo.Add(anexo);
                                }
                            }

                            // Prepara rodape
                            ASSINANTE assi = mg.ASSINANTE;

                            footer = footer.Replace("{Assinatura}", assi.ASSI_NM_NOME);

                            // Monta Mensagem
                            NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                            Email mensagem = new Email();
                            mensagem.ASSUNTO = mg.MENS_NM_CAMPANHA != null ? mg.MENS_NM_CAMPANHA : "Assunto Diverso";
                            mensagem.DEFAULT_CREDENTIALS = false;
                            mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                            mensagem.ENABLE_SSL = true;
                            mensagem.NOME_EMISSOR = assi.ASSI_NM_NOME;
                            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                            mensagem.NETWORK_CREDENTIAL = net;
                            mensagem.ATTACHMENT = listaAnexo;

                            // Checa cabeçalho e envia
                            if (header.Contains("{Nome}"))
                            {
                                foreach (CLIENTE item in listaCli)
                                {
                                    // Prepara cabeçalho
                                    header = header.Replace("{Nome}", item.CLIE_NM_NOME);
                                    String emailBody = header + body + footer;

                                    // Monta e-mail
                                    mensagem.CORPO = emailBody;
                                    mensagem.EMAIL_TO_DESTINO = item.CLIE_NM_EMAIL;

                                    // Envia mensagem
                                    try
                                    {
                                        Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);

                                        controlError.IsOK = true;
                                    }
                                    catch (Exception ex)
                                    {
                                        controlError.HandleExeption(ex);
                                        if (ex.GetType() == typeof(SmtpFailedRecipientException))
                                        {
                                            var se = (SmtpFailedRecipientException)ex;
                                            controlError.HandleExeption(se.FailedRecipient);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                String emailBody = header + body + footer;
                                mensagem.CORPO = emailBody;

                                MailAddressCollection col = new MailAddressCollection();
                                foreach (CLIENTE item in listaCli)
                                {
                                    col.Add(item.CLIE_NM_EMAIL);
                                }

                                // Envia mensagem
                                try
                                {
                                    Int32 voltaMail = CommunicationPackage.SendEmailCollection(mensagem, col);

                                    controlError.IsOK = true;
                                }
                                catch (Exception ex)
                                {
                                    controlError.HandleExeption(ex);

                                    if (ex.GetType() == typeof(SmtpFailedRecipientException))
                                    {
                                        var se = (SmtpFailedRecipientException)ex;

                                        controlError.HandleExeption(se.FailedRecipient);
                                    }
                                }
                            }

                            // Loop de retorno
                            foreach (CLIENTE item in listaCli)
                            {
                                // Grava mensagem/destino e erros
                                if (controlError.IsOK)
                                {
                                    String cab = Regex.Replace(header, "<.*?>", String.Empty);
                                    String cor = Regex.Replace(body, "<.*?>", String.Empty);
                                    String foot = Regex.Replace(footer, "<.*?>", String.Empty);
                                    String tex = cab + " " + cor + " " + foot;
                                    tex = tex.Replace("\r\n", " ");

                                    MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                                    dest.MEDE_IN_ATIVO = 1;
                                    dest.MEDE_IN_POSICAO = 1;
                                    dest.MEDE_IN_STATUS = 1;
                                    dest.CLIE_CD_ID = item.CLIE_CD_ID;
                                    dest.MEDE_DS_ERRO_ENVIO = controlError.GetMenssage();
                                    dest.MENS_CD_ID = mg.MENS_CD_ID;
                                    mg.MENSAGENS_DESTINOS.Add(dest);
                                    mg.MENS_DT_ENVIO = DateTime.Now;
                                    mg.MENS_TX_TEXTO = body;
                                    mg.MENS_NM_CABECALHO = header;
                                    mg.MENS_NM_RODAPE = footer;
                                    mg.MENS_TX_TEXTO_LIMPO = tex;
                                    mg.MENS_IN_STATUS = (int)Enumerador.StatusEnvioEmail.ENVIADO;

                                }
                                else
                                {
                                    mg.MENS_TX_RETORNO = controlError.GetMenssage();
                                    mg.MENS_IN_STATUS = (int)Enumerador.StatusEnvioEmail.ERRO;
                                }
                            }
                        }
                    }
                    // Normal
                    else
                    {
                        // Prepara inicial
                        String body = String.Empty;
                        String header = String.Empty;
                        String footer = String.Empty;
                        String link = String.Empty;
                        body = mg.MENS_TX_TEXTO;
                        header = mg.MENS_NM_CABECALHO;
                        footer = mg.MENS_NM_RODAPE;
                        link = mg.MENS_NM_LINK;

                        // Prepara rodape
                        ASSINANTE assi = mg.ASSINANTE;
                        footer = footer.Replace("{Assinatura}", assi.ASSI_NM_NOME);

                        // Trata corpo
                        StringBuilder str = new StringBuilder();
                        str.AppendLine(body);

                        // Trata link
                        if (!String.IsNullOrEmpty(link))
                        {
                            if (!link.Contains("www."))
                            {
                                link = "www." + link;
                            }
                            if (!link.Contains("http://"))
                            {
                                link = "http://" + link;
                            }
                            str.AppendLine("<a href='" + link + "'>Clique aqui para maiores informações</a>");
                        }
                        body = str.ToString();

                        // Checa e monta anexos
                        List<System.Net.Mail.Attachment> listaAnexo = new List<System.Net.Mail.Attachment>();
                        if (mg.MENSAGEM_ANEXO.Count > 0)
                        {
                            foreach (MENSAGEM_ANEXO item in mg.MENSAGEM_ANEXO)
                            {
                                String fn = server.MapPath(item.MEAN_AQ_ARQUIVO);
                                System.Net.Mail.Attachment anexo = new System.Net.Mail.Attachment(fn);
                                listaAnexo.Add(anexo);
                            }
                        }

                        // Monta Mensagem
                        NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
                        Email mensagem = new Email();
                        mensagem.ASSUNTO = mg.MENS_NM_CAMPANHA != null ? mg.MENS_NM_CAMPANHA : "Assunto Diverso";
                        mensagem.DEFAULT_CREDENTIALS = false;
                        mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
                        mensagem.ENABLE_SSL = true;
                        mensagem.NOME_EMISSOR = assi.ASSI_NM_NOME;
                        mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
                        mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
                        mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
                        mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
                        mensagem.NETWORK_CREDENTIAL = net;
                        mensagem.ATTACHMENT = listaAnexo;

                        String emailBody = header + body + footer;
                        mensagem.CORPO = emailBody;

                        MailAddressCollection col = new MailAddressCollection();
                        foreach (CLIENTE item in listaCli)
                        {
                            col.Add(item.CLIE_NM_EMAIL);
                        }

                        // Envia mensagem
                        try
                        {
                            Int32 voltaMail = CommunicationPackage.SendEmailCollection(mensagem, col);

                            controlError.IsOK = true;
                        }
                        catch (Exception ex)
                        {
                            controlError.HandleExeption(ex);

                            if (ex.GetType() == typeof(SmtpFailedRecipientException))
                            {
                                var se = (SmtpFailedRecipientException)ex;

                                controlError.HandleExeption(se.FailedRecipient);
                            }
                        }

                        // Loop de retorno
                        foreach (CLIENTE item in listaCli)
                        {
                            // Grava mensagem/destino e erros
                            if (controlError.IsOK)
                            {
                                String cab = Regex.Replace(header, "<.*?>", String.Empty);
                                String cor = Regex.Replace(body, "<.*?>", String.Empty);
                                String foot = Regex.Replace(footer, "<.*?>", String.Empty);
                                String tex = cab + " " + cor + " " + foot;
                                tex = tex.Replace("\r\n", " ");

                                MENSAGENS_DESTINOS dest = new MENSAGENS_DESTINOS();
                                dest.MEDE_IN_ATIVO = 1;
                                dest.MEDE_IN_POSICAO = 1;
                                dest.MEDE_IN_STATUS = 1;
                                dest.CLIE_CD_ID = item.CLIE_CD_ID;
                                dest.MEDE_DS_ERRO_ENVIO = controlError.GetMenssage();
                                dest.MENS_CD_ID = mg.MENS_CD_ID;
                                mg.MENSAGENS_DESTINOS.Add(dest);
                                mg.MENS_DT_ENVIO = DateTime.Now;
                                mg.MENS_TX_TEXTO = body;
                                mg.MENS_NM_CABECALHO = header;
                                mg.MENS_NM_RODAPE = footer;
                                mg.MENS_TX_TEXTO_LIMPO = tex;

                            }
                            else
                            {
                                mg.MENS_TX_RETORNO = controlError.GetMenssage();

                            }
                        }

                    }
                }catch(Exception ex)
                {
                    controlError.IsOK = false;
                    controlError.HandleExeption(ex);                        
                }

                if(!controlError.IsOK)
                {
                    mg.MENS_IN_STATUS = (int)Enumerador.StatusEnvioEmail.ERRO;
                    mg.MENS_TX_RETORNO = controlError.GetMenssage();
                }

                _mensagemRepository.Update(mg);
            }
            
            
        }
    }
}
