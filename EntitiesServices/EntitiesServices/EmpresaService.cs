using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class EmpresaService : ServiceBase<EMPRESA>, IEmpresaService
    {
        private readonly IEmpresaRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IMaquinaRepository _maqRepository;
        private readonly IRegimeTributarioRepository _regRepository;
        private readonly IEmpresaAnexoRepository _anexoRepository;
        private readonly IEmpresaMaquinaRepository _emaqRepository;
        protected Db_PrecificacaoEntities Db = new Db_PrecificacaoEntities();

        public EmpresaService(IEmpresaRepository baseRepository, ILogRepository logRepository, IMaquinaRepository maqRepository, IRegimeTributarioRepository regRepository, IEmpresaAnexoRepository anexoRepository, IEmpresaMaquinaRepository emaqRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _maqRepository = maqRepository;
            _regRepository = regRepository;
            _anexoRepository = anexoRepository;
            _emaqRepository = emaqRepository;
        }

        public EMPRESA GetItemById(Int32 id)
        {
            EMPRESA item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<EMPRESA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<EMPRESA> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public EMPRESA CheckExist(EMPRESA conta, Int32 idAss)
        {
            EMPRESA item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public EMPRESA_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public List<EMPRESA> ExecuteFilter(String nome, Int32? idAss)
        {
            List<EMPRESA> lista = _baseRepository.ExecuteFilter(nome, idAss);
            return lista;
        }

        public List<MAQUINA> GetAllMaquinas(Int32 idAss)
        {
            return _maqRepository.GetAllItens(idAss);
        }

        public List<REGIME_TRIBUTARIO> GetAllRegimes()
        {
            return _regRepository.GetAllItens();
        }

        public Int32 Create(EMPRESA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Create(EMPRESA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }


        public Int32 Edit(EMPRESA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    EMPRESA obj = _baseRepository.GetById(item.EMPR_CD_ID);
                    _baseRepository.Detach(obj);
                    _logRepository.Add(log);
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Edit(EMPRESA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    EMPRESA obj = _baseRepository.GetById(item.EMPR_CD_ID);
                    _baseRepository.Detach(obj);
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Delete(EMPRESA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Remove(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public EMPRESA_MAQUINA GetByEmpresaMaquina(Int32 empresa, Int32 maquina)
        {
            return _emaqRepository.GetByEmpresaMaquina(empresa, maquina);
        }

        public EMPRESA_MAQUINA GetMaquinaById(Int32 id)
        {
            return _emaqRepository.GetItemById(id);
        }

        public REGIME_TRIBUTARIO GetRegimeById(Int32 id)
        {
            return _regRepository.GetItemById(id);
        }

        public Int32 EditMaquina(EMPRESA_MAQUINA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    EMPRESA_MAQUINA obj = _emaqRepository.GetById(item.EMMA_CD_ID);
                    _emaqRepository.Detach(obj);
                    _emaqRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreateMaquina(EMPRESA_MAQUINA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _emaqRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }



    }
}
