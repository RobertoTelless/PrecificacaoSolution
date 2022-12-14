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
    public class EmpresaAppService : AppServiceBase<EMPRESA>, IEmpresaAppService
    {
        private readonly IEmpresaService _baseService;

        public EmpresaAppService(IEmpresaService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public EMPRESA CheckExist(EMPRESA conta, Int32 idAss)
        {
            EMPRESA item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<EMPRESA> GetAllItens(Int32 idAss)
        {
            List<EMPRESA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<EMPRESA> GetAllItensAdm(Int32 idAss)
        {
            List<EMPRESA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public EMPRESA GetItemById(Int32 id)
        {
            EMPRESA item = _baseService.GetItemById(id);
            return item;
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

        public EMPRESA_ANEXO GetAnexoById(Int32 id)
        {
            EMPRESA_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public EMPRESA_MAQUINA GetMaquinaById(Int32 id)
        {
            EMPRESA_MAQUINA lista = _baseService.GetMaquinaById(id);
            return lista;
        }

        public EMPRESA_PLATAFORMA GetPlataformaById(Int32 id)
        {
            EMPRESA_PLATAFORMA lista = _baseService.GetPlataformaById(id);
            return lista;
        }

        public REGIME_TRIBUTARIO GetRegimeById(Int32 id)
        {
            REGIME_TRIBUTARIO lista = _baseService.GetRegimeById(id);
            return lista;
        }

        public Int32 ExecuteFilter(String nome, Int32 idAss, out List<EMPRESA> objeto)
        {
            try
            {
                objeto = new List<EMPRESA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(nome, idAss);
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

        public List<MAQUINA> GetAllMaquinas(Int32 idAss)
        {
            return _baseService.GetAllMaquinas(idAss);
        }

        public List<REGIME_TRIBUTARIO> GetAllRegimes()
        {
            return _baseService.GetAllRegimes();
        }

        public Int32 ValidateCreate(EMPRESA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia pr??via
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.EMPR_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                item.EMPR_DT_CADASTRO = DateTime.Today.Date;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddEMPR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<EMPRESA>(item)
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

        public Int32 ValidateEdit(EMPRESA item, EMPRESA itemAntes, USUARIO usuario)
        {
            try
            {
                // Criticas
                if (item.EMPR_PC_VENDA_CREDITO + item.EMPR_PC_VENDA_DEBITO + item.EMPR_PC_VENDA_DINHEIRO > 100)
                {
                    return 1;
                }
                if (item.EMPR_PC_VENDA_CREDITO + item.EMPR_PC_VENDA_DEBITO + item.EMPR_PC_VENDA_DINHEIRO < 100)
                {
                    return 2;
                }

                // Calcula taxa m??dia credito
                EMPRESA emp = _baseService.GetItemById(usuario.ASSI_CD_ID);
                List<EMPRESA_MAQUINA> maq = emp.EMPRESA_MAQUINA.ToList();
                Decimal somaC = 0;
                Decimal somaD = 0;
                Decimal taxaC = 0;
                Decimal taxaD = 0;
                Int32 quant = 0;
                foreach (EMPRESA_MAQUINA em in maq)
                {
                    somaC += em.MAQUINA.MAQN_PC_CREDITO;
                    somaD += em.MAQUINA.MAQN_PC_DEBITO;
                    quant++;
                }
                taxaC = somaC / quant;
                taxaD = somaD / quant;
                item.EMPR_VL_TAXA_MEDIA = taxaC;
                item.EMPR_VL_TAXA_MEDIA_DEBITO = taxaD;

                // Monta Log                
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditEMPR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<EMPRESA>(item),
                    LOG_TX_TEXTO_ANTES = Serialization.SerializeJSON<EMPRESA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(EMPRESA item, EMPRESA itemAntes)
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

        public Int32 ValidateDelete(EMPRESA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.PRODUTO_ESTOQUE_EMPRESA.Count > 0)
                {
                    return 1;
                }
                if (item.PRODUTO_TABELA_PRECO.Count > 0)
                {
                    return 2;
                }

                // Acerta campos
                item.EMPR_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleEMPR",
                    LOG_TX_TEXTO = "Empresa: " + item.EMPR_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(EMPRESA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.EMPR_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatEMPR",
                    LOG_TX_TEXTO = "Empresa: " + item.EMPR_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditMaquina(EMPRESA_MAQUINA item)
        {
            try
            {
                // Persiste
                return _baseService.EditMaquina(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateMaquina(EMPRESA_MAQUINA item, Int32 idAss)
        {
            try
            {
                // Verifica existencia
                EMPRESA_MAQUINA volta1 = _baseService.CheckExistMaquina(item, idAss);
                if (volta1 != null)
                {
                    return 1;
                }

                // Persiste
                Int32 volta = _baseService.CreateMaquina(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public EMPRESA_MAQUINA GetByEmpresaMaquina(Int32 empresa, Int32 maquina)
        {
            return _baseService.GetByEmpresaMaquina(empresa, maquina);
        }

        public Int32 ValidateEditPlataforma(EMPRESA_PLATAFORMA item)
        {
            try
            {
                // Persiste
                return _baseService.EditPlataforma(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreatePlataforma(EMPRESA_PLATAFORMA item, Int32 idAss)
        {
            try
            {
                // Verifica existencia
                EMPRESA_PLATAFORMA volta1 = _baseService.CheckExistPlataforma(item, idAss);
                if (volta1 != null)
                {
                    return 1;
                }

                // Persiste
                Int32 volta = _baseService.CreatePlataforma(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public EMPRESA_PLATAFORMA GetByEmpresaPlataforma(Int32 empresa, Int32 plataforma)
        {
            return _baseService.GetByEmpresaPlataforma(empresa, plataforma);
        }

    }
}
