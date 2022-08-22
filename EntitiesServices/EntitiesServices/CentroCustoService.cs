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
    public class CentroCustoService : ServiceBase<PLANO_CONTA>, ICentroCustoService
    {
        private readonly ICentroCustoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        protected Db_PrecificacaoEntities Db = new Db_PrecificacaoEntities();

        public CentroCustoService(ICentroCustoRepository baseRepository, ILogRepository logRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;

        }

        public PLANO_CONTA GetItemById(Int32 id)
        {
            PLANO_CONTA item = _baseRepository.GetItemById(id);
            return item;
        }

        public PLANO_CONTA CheckExist(PLANO_CONTA obj, Int32 idAss)
        {
            PLANO_CONTA item = _baseRepository.CheckExist(obj, idAss);
            return item;
        }

        public List<PLANO_CONTA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<PLANO_CONTA> GetAllReceitas(Int32 idAss)
        {
            return _baseRepository.GetAllReceitas(idAss);
        }

        public List<PLANO_CONTA> GetAllDespesas(Int32 idAss)
        {
            return _baseRepository.GetAllDespesas(idAss);
        }

        public List<PLANO_CONTA> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<PLANO_CONTA> ExecuteFilter(Int32? grupoId, Int32? subGrupoId, Int32? tipo, Int32? movimento, String numero, String nome, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(grupoId, subGrupoId, tipo, movimento, numero, nome, idAss);

        }

        public Int32 Create(PLANO_CONTA item, LOG log)
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

        public Int32 Create(PLANO_CONTA item)
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


        public Int32 Edit(PLANO_CONTA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PLANO_CONTA obj = _baseRepository.GetById(item.CECU_CD_ID);
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

        public Int32 Edit(PLANO_CONTA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PLANO_CONTA obj = _baseRepository.GetById(item.CECU_CD_ID);
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

        public Int32 Delete(PLANO_CONTA item, LOG log)
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
    }
}
