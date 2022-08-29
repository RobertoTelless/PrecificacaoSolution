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

namespace ApplicationServices.Services
{
    public class ClienteAppService : AppServiceBase<CLIENTE>, IClienteAppService
    {
        private readonly IClienteService _baseService;
        private readonly IConfiguracaoService _confService;

        public ClienteAppService(IClienteService baseService, IConfiguracaoService confService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
        }

        public List<CLIENTE> GetAllItens(Int32 idAss)
        {
            List<CLIENTE> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public CLIENTE_ANOTACAO GetComentarioById(Int32 id)
        {
            return _baseService.GetComentarioById(id);
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

        public List<CLIENTE> GetAllItensAdm(Int32 idAss)
        {
            List<CLIENTE> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public List<TIPO_CONTRIBUINTE> GetAllContribuinte(Int32 idAss)
        {
            List<TIPO_CONTRIBUINTE> lista = _baseService.GetAllContribuinte(idAss);
            return lista;
        }

        public List<REGIME_TRIBUTARIO> GetAllRegimes(Int32 idAss)
        {
            List<REGIME_TRIBUTARIO> lista = _baseService.GetAllRegimes(idAss);
            return lista;
        }

        public List<SEXO> GetAllSexo()
        {
            List<SEXO> lista = _baseService.GetAllSexo();
            return lista;
        }

        public CLIENTE GetItemById(Int32 id)
        {
            CLIENTE item = _baseService.GetItemById(id);
            return item;
        }

        public CLIENTE GetByEmail(String email)
        {
            CLIENTE item = _baseService.GetByEmail(email);
            return item;
        }

        public CLIENTE CheckExist(CLIENTE conta, Int32 idAss)
        {
            CLIENTE item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<CATEGORIA_CLIENTE> GetAllTipos(Int32 idAss)
        {
            List<CATEGORIA_CLIENTE> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }

        public List<TIPO_PESSOA> GetAllTiposPessoa()
        {
            List<TIPO_PESSOA> lista = _baseService.GetAllTiposPessoa();
            return lista;
        }

        public CLIENTE_ANEXO GetAnexoById(Int32 id)
        {
            CLIENTE_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public CLIENTE_CONTATO GetContatoById(Int32 id)
        {
            CLIENTE_CONTATO lista = _baseService.GetContatoById(id);
            return lista;
        }

        public CLIENTE_REFERENCIA GetReferenciaById(Int32 id)
        {
            CLIENTE_REFERENCIA lista = _baseService.GetReferenciaById(id);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? id, Int32? catId, String razao, String nome, String cpf, String cnpj, String email, String cidade, Int32? uf, Int32? ativo, Int32 idAss, out List<CLIENTE> objeto)
        {
            try
            {
                objeto = new List<CLIENTE>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(id, catId, razao, nome, cpf, cnpj, email, cidade, uf, ativo, idAss);
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

        public Int32 ValidateCreate(CLIENTE item, USUARIO usuario)
        {
            try
            {
                // Recupera flag de duplicidade
                CONFIGURACAO conf = _confService.GetItemById(usuario.ASSI_CD_ID);
                Int32? dup = conf.CONF_IN_CNPJ_DUPLICADO;

                // Verifica Existencia
                if (item.TIPE_CD_ID == 1)
                {
                    if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                    {
                        return 1;
                    }
                }
                if (item.TIPE_CD_ID == 2)
                {
                    if (dup == 0)
                    {
                        if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                        {
                            return 1;
                        }
                    }
                }

                // Completa objeto
                item.CLIE_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCLIE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<CLIENTE>(item)
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

        public Int32 ValidateEdit(CLIENTE item, CLIENTE itemAntes, USUARIO usuario)
        {
            try
            {
                if (itemAntes.ASSINANTE != null)
                {
                    itemAntes.ASSINANTE = null;
                }
                if (itemAntes.CATEGORIA_CLIENTE != null)
                {
                    itemAntes.CATEGORIA_CLIENTE = null;
                }
                if (itemAntes.EMPRESA != null)
                {
                    itemAntes.EMPRESA = null;
                }
                if (itemAntes.SEXO != null)
                {
                    itemAntes.SEXO = null;
                }
                if (itemAntes.TIPO_CONTRIBUINTE != null)
                {
                    itemAntes.TIPO_CONTRIBUINTE = null;
                }
                if (itemAntes.TIPO_PESSOA != null)
                {
                    itemAntes.TIPO_PESSOA = null;
                }
                if (itemAntes.UF != null)
                {
                    itemAntes.UF = null;
                }
                if (itemAntes.USUARIO != null)
                {
                    itemAntes.USUARIO = null;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditCLIE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<CLIENTE>(item),
                    LOG_TX_TEXTO_ANTES = Serialization.SerializeJSON<CLIENTE>(itemAntes)
                };

                // Persiste
                item.TIPO_PESSOA = null;
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CLIENTE item, CLIENTE itemAntes)
        {
            try
            {
                item.UF = null;
                item.TIPO_PESSOA = null;
                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CLIENTE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (true)
                {

                }

                // Acerta campos
                item.CLIE_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCLIE",
                    LOG_TX_TEXTO = "Nome: " + item.CLIE_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CLIENTE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CLIE_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCLIE",
                    LOG_TX_TEXTO = "Nome: " + item.CLIE_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditContato(CLIENTE_CONTATO item)
        {
            try
            {
                // Persiste
                return _baseService.EditContato(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateContato(CLIENTE_CONTATO item)
        {
            try
            {
                item.CLCO_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateContato(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditReferencia(CLIENTE_REFERENCIA item)
        {
            try
            {
                // Persiste
                return _baseService.EditReferencia(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateReferencia(CLIENTE_REFERENCIA item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateReferencia(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
