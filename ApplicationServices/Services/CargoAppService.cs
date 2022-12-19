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
    public class CargoAppService : AppServiceBase<CARGO_USUARIO>, ICargoAppService
    {
        private readonly ICargoService _baseService;

        public CargoAppService(ICargoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public CARGO_USUARIO CheckExist(CARGO_USUARIO conta, Int32 idAss)
        {
            CARGO_USUARIO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<CARGO_USUARIO> GetAllItens(Int32 idAss)
        {
            List<CARGO_USUARIO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<CARGO_USUARIO> GetAllItensAdm(Int32 idAss)
        {
            List<CARGO_USUARIO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public CARGO_USUARIO GetItemById(Int32 id)
        {
            CARGO_USUARIO item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(CARGO_USUARIO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.CARG_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCARG",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<CARGO_USUARIO>(item)
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

        public Int32 ValidateEdit(CARGO_USUARIO item, CARGO_USUARIO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditCARG",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<CARGO_USUARIO>(item),
                    LOG_TX_TEXTO_ANTES = Serialization.SerializeJSON<CARGO_USUARIO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CARGO_USUARIO item, CARGO_USUARIO itemAntes)
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

        public Int32 ValidateDelete(CARGO_USUARIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.USUARIO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.CARG_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleCARG",
                    LOG_TX_TEXTO = "Cargo: " + item.CARG_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CARGO_USUARIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CARG_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCARG",
                    LOG_TX_TEXTO = "Cargo: " + item.CARG_NM_NOME
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
