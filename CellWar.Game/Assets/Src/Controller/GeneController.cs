﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CellWar.GameData;
using CellWar.Model.Map;
using CellWar.Model.Substance;
using static CellWar.Model.Substance.Strain;

namespace CellWar.Controller.Gene {
    public class CodingGeneController {
        public CodingGeneController() {
            EffectEvents.Add( ProductChemical );
            EffectEvents.Add( ConsumeAndDecomposite );
            EffectEvents.Add( ModifyPopulation );
            EffectEvents.Add( ImportChemical );
            EffectEvents.Add( StrainSpread );
        }
        public delegate bool EffectEvent( ref Strain parentStrain, ref Block currentBlock, ref CodingGene gene );
        /// <summary>
        /// Effect 函数列表
        /// </summary>
        public List<EffectEvent> EffectEvents { get; set; } = new List<EffectEvent>();
        
        /// <summary>
        /// 人口*系数 的值影响物质改变量的大小
        /// 7.18 改动?
        /// </summary>
        /// <param name="parentStrain"></param>
        /// <returns></returns>
        private float GetPopulationDelta( ref Strain parentStrain, ref CodingGene gene ) => ( parentStrain.Population * gene.PopulationCoefficient ) + gene.PopulationIntercept;
        private float GetProductionChemicalDelta( ref Strain parentStrain, ref CodingGene gene ) => ( parentStrain.Population * gene.ProductionChemicalCoeffeicient ) + gene.ProductionChemicalIntercept;
        private float GetImportChemicalDelta( ref Strain parentStrain, ref CodingGene gene ) => ( parentStrain.Population * gene.ImportChemicalCoeffeicient ) + gene.ImportChemicalIntercept;

        public bool ProductChemical( ref Strain parentStrain, ref Block currentBlock, ref CodingGene gene ) {
            var ProductionChemicalName = gene.ProductionChemicalName;
            var productionChemical = Local.FindChemicalByName( ProductionChemicalName );
            // ----- 对化学物质产生影响 -----
            // 查找是否存在这个物质
            var productChem = currentBlock.PublicChemicals.Find( che => { return che.Name == ProductionChemicalName; } );
            if( productChem == null && ProductionChemicalName != "" ) {
                productChem = new Chemical {
                    Name = ProductionChemicalName,
                    Count = 0,
                    SpreadRate = productionChemical.SpreadRate
                };
                // 向block物质集中添加改变的chemical
                currentBlock.PublicChemicals.Add( productChem );
                productChem.Count += ( int )GetProductionChemicalDelta( ref parentStrain, ref gene );
            }
            // ----- 对化学 物质产生影响 -----
            return false;
        }
        public bool ConsumeAndDecomposite( ref Strain parentStrain, ref Block currentBlock, ref CodingGene gene ) {
            string ConsumeChemicalName = gene.ConsumeChemicalName.Clone() as string;
            // ----- 消耗 -----
            // 若小号物质不存在，gene罢工
            var consumeChemical = currentBlock.PublicChemicals.Find( chem => { return chem.Name == ConsumeChemicalName; } );
            var chemicalToConsume = ( gene.IsConsumePublic ? currentBlock.PublicChemicals : parentStrain.PrivateChemicals ).Find( chem => { return chem.Name == ConsumeChemicalName; } );
            if( chemicalToConsume == null ) {
                if( gene.ConsumeChemicalName == "" ) {
                    return false;
                }
                return false; // 根本不存在该物质，不工作
            } else {
                if( chemicalToConsume.Count >= gene.ConsumeChemicalCount ) {
                    // ----- 分解 -----
                    // 若小号物质不存在，gene罢工
                    var decompositeChemical = currentBlock.PublicChemicals.Find( chem => { return chem.Name == ConsumeChemicalName; } );
                    var chemicalToDecomposite = ( gene.IsDecompositionPublic ? currentBlock.PublicChemicals : parentStrain.PrivateChemicals ).Find( chem => { return chem.Name == ConsumeChemicalName; } );
                    if( chemicalToDecomposite == null ) {
                        return false; // 根本不存在该物质，不工作
                    } else {
                        if( chemicalToDecomposite.Count >= gene.DecompositionChemicalCount ) {
                            chemicalToDecomposite.Count -= gene.DecompositionChemicalCount;
                            chemicalToConsume.Count -= gene.ConsumeChemicalCount;
                        } else {
                            return false; // 需要消耗的量不足，不工作
                        }
                    }
                    // ----- 分解 -----
                } else {
                    return false; // 需要消耗的量不足，不工作
                }
            }
            return true;
            // ----- 消耗 -----
        }
        public bool ModifyPopulation( ref Strain parentStrain, ref Block currentBlock, ref CodingGene gene ) {
            var delta = ( int )GetPopulationDelta( ref parentStrain, ref gene );
            if( MainGameCurrent.BlockController.IsPopulationBeingFull( currentBlock, delta )
                && !MainGameCurrent.BlockController.IsPopulationFull( currentBlock ) ) { // 最后一次人口增加，即将变满
                parentStrain.Population += currentBlock.Capacity - MainGameCurrent.BlockController.GetTotalPopulation( currentBlock );
            } else if( MainGameCurrent.BlockController.IsPopulationFull( currentBlock ) ) { // 人口已满，无法增加
                return false;
            } else {
                parentStrain.Population += delta;
            }
            return true;
        }
        public bool ImportChemical( ref Strain parentStrain, ref Block currentBlock, ref CodingGene gene ) {
            var ImportChemicalName = gene.ImportChemicalName;
            var ProductionChemicalCount = gene.ProductionChemicalCount;
            // --- 添加私有化学库的量 ---
            // 先寻找block内是否存在该种化学物质
            var importChemical = Local.FindChemicalByName( ImportChemicalName );
            var publicChemical = currentBlock.PublicChemicals.Find( chem => { return chem.Name == ImportChemicalName; } );
            if( publicChemical != null ) {
                var privateChemical = parentStrain.PrivateChemicals.Find( chem => { return chem.Name == publicChemical.Name; } );
                if( privateChemical == null ) {
                    parentStrain.PrivateChemicals.Add( new Chemical {
                        Count = 0,
                        Name = ImportChemicalName,
                        SpreadRate = importChemical.SpreadRate
                    } ); // 如果没有，先添加
                }
                var importCount = ( int )GetPopulationDelta( ref parentStrain, ref gene ) + ProductionChemicalCount;
                if( publicChemical.Count >= privateChemical.Count ) {
                    privateChemical.Count += ( int )GetImportChemicalDelta( ref parentStrain, ref gene );
                    publicChemical.Count -= ( int )GetImportChemicalDelta( ref parentStrain, ref gene );
                }
            }
            return true;
            // ----- 对父strain产生影响 -----

        }
        public bool StrainSpread( ref Strain parentStrain, ref Block currentBlock, ref CodingGene gene ) {
            var SpreadConditionRate = gene.SpreadConditionRate;
            // ----- 细菌扩散 -----
            // 是否满足扩散条件
            if( parentStrain.Population >= currentBlock.Capacity * SpreadConditionRate ) {
                string strainName = parentStrain.Name.Clone() as string;
                // 为周围的格子添加该细菌
                foreach( var block in currentBlock.NeighborBlocks ) {
                    if( block.Strains.Exists( m => { return m.Name == strainName; } ) ) {
                        continue;
                    }
                    var cloneStrain = ( Strain )parentStrain.Clone();
                    // 设定初始人口数
                    cloneStrain.Population = ( int )( parentStrain.Population * gene.FirstSpreadMountRate );
                    block.Strains.Add( cloneStrain );
                }
            }
            return true;
            // ----- 细菌扩散 -----
        }
        /// <summary>
        /// 游戏核心函数
        /// </summary>
        /// <param name="parentStrain">该基因的父细菌</param>
        /// <param name="currentBlock">父细菌所在的block</param>
        public void Effect( ref Strain parentStrain, ref Block currentBlock, ref CodingGene gene ) {
            foreach( var eve in EffectEvents ) {
                if( !eve( ref parentStrain, ref currentBlock, ref gene ) ) return;
            }
        }
    }
    public class RegulatoryGeneController {
        /// <summary>
        /// 判断格子中的chemicals是否是conditions的父集，且满足一定条件：格子中chemical的Count不小于condition中的Count
        /// </summary>
        /// <param name="chemicalsInBlock"></param>
        /// <returns></returns>
        public bool IsTriggered( List<Model.Substance.Chemical> chemicalsInBlock, RegulatoryGene reg ) {
            switch( reg.Type ) {
                case "PA": return isMeetAllCondition( chemicalsInBlock, reg );
                case "NA": return !isMeetAllCondition( chemicalsInBlock, reg );
                case "PO": return isMeetAtLeastOneCondition( chemicalsInBlock, reg );
                case "NO": return !isMeetAtLeastOneCondition( chemicalsInBlock, reg );
                case "TRUE": return true;
                case "FALSE": return false;
                default: return default;
            }
        }
        #region PRIVATE
        /// <summary>
        /// 判断是否满足所有条件
        /// </summary>
        /// <param name="chemicalsInBlock"></param>
        /// <returns></returns>
        protected bool isMeetAllCondition( List<Model.Substance.Chemical> chemicalsInBlock, RegulatoryGene reg ) {
            foreach( var cInCondition in reg.Conditions ) {
                // 寻找格子中是否含有条件的chemical
                var result = chemicalsInBlock.Find( r => { return r.Name == cInCondition.Name; } );
                // 如果不存在直接条件直接不成立
                if( result == null ) {
                    return false;
                }
                // 如果存在，判断数量是否达标，如不达标直接不成立
                if( cInCondition.Count > 0 ) {
                    if( result.Count < cInCondition.Count ) {
                        return false;
                    }
                } else if( cInCondition.Count < 0 ) {
                    if( result.Count > -cInCondition.Count ) {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 仅有一个条件就触发
        /// </summary>
        /// <param name="chemicalsInBlock"></param>
        /// <returns></returns>
        protected bool isMeetAtLeastOneCondition( List<Model.Substance.Chemical> chemicalsInBlock, RegulatoryGene reg ) {
            foreach( var cInCondition in reg.Conditions ) {
                // 寻找格子中是否含有条件的chemical
                var result = chemicalsInBlock.Find( r => { return r.Name == cInCondition.Name; } );
                // 如果不存在直接条件直接不成立
                if( result == null ) {
                    return false;
                }
                // 如果存在，判断数量是否达标，如不达标直接不成立
                if( result.Count < cInCondition.Count ) {
                    return false;
                }
                return true;
            }
            return true; // 不应该到达这里
        }
        #endregion

        #region MEANINGFUL_REGULARTOYGENES**DEPED
        ///// <summary>
        ///// 正全调控基因
        ///// 必须满足所有条件方可触发条件
        ///// </summary>
        //public class PositiveAllRegulatoryGene : RegulatoryGene {
        //    public sealed override bool IsTriggered( List<Substance.Chemical> chemicalsInBlock ) {
        //    }
        //}
        ///// <summary>
        ///// 负全调控基因
        ///// 当满足所有条件是关闭条件触发
        ///// </summary>
        //public class NegativeAllRegulartoryGene : RegulatoryGene {
        //    public sealed override bool IsTriggered( List<Substance.Chemical> chemicalsInBlock ) {
        //        return !isMeetAllCondition( chemicalsInBlock );
        //    }
        //}

        ///// <summary>
        ///// 正或调控基因
        ///// </summary>
        //public class PositiveOrRegulartoryGene : RegulatoryGene {
        //    public sealed override bool IsTriggered( List<Substance.Chemical> chemicalsInBlock ) {
        //        return isMeetAtLeastOneCondition( chemicalsInBlock );
        //    }
        //}

        ///// <summary>
        ///// 负或调控基因
        ///// </summary>
        //public class NegativeOrRegulartoryGene : RegulatoryGene {
        //    public sealed override bool IsTriggered( List<Substance.Chemical> chemicalsInBlock ) {
        //        return isMeetAllCondition( chemicalsInBlock );
        //    }
        //}

        //public class TrueRegGene : RegulatoryGene {
        //    public sealed override bool IsTriggered( List<Chemical> chemicalsInBlock ) {
        //        return true;
        //    }
        //}
        #endregion
    }
}
