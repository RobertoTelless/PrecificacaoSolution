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
    public class SubgrupoService : ServiceBase<SUBGRUPO_PLANO_CONTA>, ISubgrupoService
    {
        private readonly ISubgrupoRepository _baseRepository;
        private readonly IGrupoCCRepository _gruRepository;
        private readonly ILogRepository _logRepository;
        protected Db_PrecificacaoEntities Db = new Db_PrecificacaoEntities();

        public SubgrupoService(ISubgrupoRepository baseRepository, ILogRepository logRepository, IGrupoCCRepository gruRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _gruRepository = gruRepository;
        }

        public SUBGRUPO_PLANO_CONTA GetItemById(Int32 id)
        {
            SUBGRUPO_PLANO_CONTA item = _baseRepository.GetItemById(id);
            return item;
        }

        public SUBGRUPO_PLANO_CONTA CheckExist(SUBGRUPO_PLANO_CONTA obj, Int32 idAss)
        {
            SUBGRUPO_PLANO_CONTA item = _baseRepository.CheckExist(obj, idAss);
            return item;
        }

        public List<SUBGRUPO_PLANO_CONTA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<SUBGRUPO_PLANO_CONTA> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<GRUPO_PLANO_CONTA> GetAllGrupos(Int32 idAss)
        {
            return _gruRepository.GetAllItens(idAss);
        }

        public Int32 Create(SUBGRUPO_PLANO_CONTA item, LOG log)
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

        public Int32 Create(SUBGRUPO_PLANO_CONTA item)
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


        public Int32 Edit(SUBGRUPO_PLANO_CONTA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    SUBGRUPO_PLANO_CONTA obj = _baseRepository.GetById(item.SGCC_CD_ID);
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

        public Int32 Edit(SUBGRUPO_PLANO_CONTA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    SUBGRUPO_PLANO_CONTA obj = _baseRepository.GetById(item.SGCC_CD_ID);
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

        public Int32 Delete(SUBGRUPO_PLANO_CONTA item, LOG log)
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
