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
    public class FormaPagRecAppService : AppServiceBase<FORMA_PAGTO_RECTO>, IFormaPagRecAppService
    {
        private readonly IFormaPagRecService _baseService;

        public FormaPagRecAppService(IFormaPagRecService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public FORMA_PAGTO_RECTO CheckExist(FORMA_PAGTO_RECTO conta, Int32 idAss)
        {
            FORMA_PAGTO_RECTO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<FORMA_PAGTO_RECTO> GetAllItens(Int32 idAss)
        {
            List<FORMA_PAGTO_RECTO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<FORMA_PAGTO_RECTO> GetAllItensAdm(Int32 idAss)
        {
            List<FORMA_PAGTO_RECTO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public FORMA_PAGTO_RECTO GetItemById(Int32 id)
        {
            FORMA_PAGTO_RECTO item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ExecuteFilter(Int32? tipo, Int32? conta, String nome, Int32? idAss, out List<FORMA_PAGTO_RECTO> objeto)
        {
            try
            {
                objeto = new List<FORMA_PAGTO_RECTO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(tipo, conta, nome, idAss);
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

        public List<CONTA_BANCO> GetAllContas(Int32 idAss)
        {
            return _baseService.GetAllContas(idAss);
        }

        public Int32 ValidateCreate(FORMA_PAGTO_RECTO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.FOPR_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                item.FOPR_DT_CADASTRO = DateTime.Today.Date;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddFOPR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<FORMA_PAGTO_RECTO>(item)
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

        public Int32 ValidateEdit(FORMA_PAGTO_RECTO item, FORMA_PAGTO_RECTO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditFOPR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<FORMA_PAGTO_RECTO>(item),
                    LOG_TX_TEXTO_ANTES = Serialization.SerializeJSON<FORMA_PAGTO_RECTO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(FORMA_PAGTO_RECTO item, FORMA_PAGTO_RECTO itemAntes)
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

        public Int32 ValidateDelete(FORMA_PAGTO_RECTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.FOPR_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleFOPR",
                    LOG_TX_TEXTO = "Forma: " + item.FOPR_NM_NOME_FORMA
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(FORMA_PAGTO_RECTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.FOPR_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatFOPR",
                    LOG_TX_TEXTO = "Forma: " + item.FOPR_NM_NOME_FORMA
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
