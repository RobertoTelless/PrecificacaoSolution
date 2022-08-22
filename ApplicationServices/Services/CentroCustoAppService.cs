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
    public class CentroCustoAppService : AppServiceBase<PLANO_CONTA>, ICentroCustoAppService
    {
        private readonly ICentroCustoService _baseService;

        public CentroCustoAppService(ICentroCustoService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<PLANO_CONTA> GetAllItens(Int32 idAss)
        {
            List<PLANO_CONTA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<PLANO_CONTA> GetAllDespesas(Int32 idAss)
        {
            List<PLANO_CONTA> lista = _baseService.GetAllDespesas(idAss);
            return lista;
        }

        public List<PLANO_CONTA> GetAllReceitas(Int32 idAss)
        {
            List<PLANO_CONTA> lista = _baseService.GetAllReceitas(idAss);
            return lista;
        }

        public List<PLANO_CONTA> GetAllItensAdm(Int32 idAss)
        {
            List<PLANO_CONTA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public PLANO_CONTA GetItemById(Int32 id)
        {
            PLANO_CONTA item = _baseService.GetItemById(id);
            return item;
        }

        public PLANO_CONTA CheckExist(PLANO_CONTA obj, Int32 idAss)
        {
            PLANO_CONTA item = _baseService.CheckExist(obj, idAss);
            return item;
        }

        public Int32 ExecuteFilter(Int32? grupoId, Int32? subGrupoId, Int32? tipo, Int32? movimento, String numero, String nome, Int32 idAss, out List<PLANO_CONTA> objeto)
        {
            try
            {
                objeto = new List<PLANO_CONTA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(grupoId, subGrupoId, tipo, movimento, numero, nome, idAss);
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

        public Int32 ValidateCreate(PLANO_CONTA item, USUARIO usuario)
        {
            try
            {
                // Critica
                List<PLANO_CONTA> lista = _baseService.GetAllItens(usuario.ASSI_CD_ID).Where(p => p.GRCC_CD_ID == item.GRCC_CD_ID & p.SGCC_CD_ID == item.SGCC_CD_ID & p.CECU_NR_NUMERO == item.CECU_NR_NUMERO).ToList();
                if (lista.Count > 0)
                {
                    return 1;
                }             
                
                // Completa objeto
                item.CECU_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCECU",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<PLANO_CONTA>(item)
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

        public Int32 ValidateEdit(PLANO_CONTA item, PLANO_CONTA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditCECU",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<PLANO_CONTA>(item),
                    LOG_TX_TEXTO_ANTES = Serialization.SerializeJSON<PLANO_CONTA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(PLANO_CONTA item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                //if (item.CONTA_PAGAR.Count > 0 | item.CONTA_RECEBER.Count > 0 | item.PEDIDO_COMPRA.Count > 0)
                //{
                //    return 1;
                //}

                // Acerta campos
                item.CECU_IN_ATIVO = 0;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCECU",
                    LOG_TX_TEXTO = "Plano de Conta: " + item.CECU_NM_EXIBE
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(PLANO_CONTA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CECU_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCECU",
                    LOG_TX_TEXTO = "Plano de Conta: " + item.CECU_NM_EXIBE
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
