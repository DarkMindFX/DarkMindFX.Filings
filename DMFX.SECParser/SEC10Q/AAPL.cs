using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMFX.Interfaces;
using System.ComponentModel.Composition;
using System.Xml;

namespace DMFX.SECParser.SEC10Q
{
    [Export("AAPL", typeof(IFilingParser))]
    public class AAPL : SECParserBase
    {
        public AAPL() : base("10-Q")
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            SECParserParams secParams = parserParams as SECParserParams;

            SECParserResult result = new SECParserResult();

            try
            {
                ValidateFile(secParams, result);
                if (result.Success)
                {
                    var doc = OpenDocument(secParams);
                    if (doc != null)
                    {
                        ExtractContexts(doc, result);
                        ExtractCompanyData(doc, result);
                        ExtractFilingData(doc, result);
                       
                        ParseStatementSection(doc, result, "CONDENSED CONSOLIDATED STATEMENTS OF OPERATIONS", statOfOpsTags);
                        ParseStatementSection(doc, result, "CONDENSED CONSOLIDATED STATEMENTS OF COMPREHENSIVE INCOME", statOfCompIncomeTags);
                        ParseStatementSection(doc, result, "CONDENSED CONSOLIDATED BALANCE SHEETS", balanceSheetTags);
                        ParseStatementSection(doc, result, "CONDENSED CONSOLIDATED STATEMENTS OF CASH FLOWS", stateOfCashFlowsTags);
                        ParseStatementSection(doc, result, "Earnings Per Share", epsTags);
                        ParseStatementSection(doc, result, "Cash, Cash Equivalents and Marketable Securities", cashMarketSecTags);
                        ParseStatementSection(doc, result, "Derivative Assets & Liabilities", derivateFinInstrAssetsLiabilitiesTags);
                        ParseStatementSection(doc, result, "Other Comprehansive Income Instruments Gain/Loss", instrumentsGainLossTags);
                        // TODO: debth and others tags
                        ParseStatementSection(doc, result, "Cash Dividents Per Share", commonStockDividentsTags);
                        ParseStatementSection(doc, result, "Reportable Operating Segments", reportableOperatingsTags);


                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.AddError(EErrorCodes.ParserError, EErrorType.Error, ex.Message);
            }

            return result;
        }

        #region Sections Tags

        string[][] statOfOpsTags = {
                new string[] { "us-gaap:SalesRevenueNet", "SalesRevenueNet" },
                new string[] { "us-gaap:CostOfGoodsAndServicesSold", "CostOfGoodsAndServicesSold" },
                new string[] { "us-gaap:GrossProfit", "GrossProfit" },
                new string[] { "us-gaap:ResearchAndDevelopmentExpense", "ResearchAndDevelopmentExpense" },
                new string[] { "us-gaap:SellingGeneralAndAdministrativeExpense", "SellingGeneralAndAdministrativeExpense" },
                new string[] { "us-gaap:OperatingExpenses", "OperatingExpenses" },
                new string[] { "us-gaap:OperatingIncomeLoss", "OperatingIncomeLoss" },
                new string[] { "us-gaap:NonoperatingIncomeExpense", "NonoperatingIncomeExpense" },
                new string[] { "us-gaap:IncomeLossFromContinuingOperationsBeforeIncomeTaxesExtraordinaryItemsNoncontrollingInterest", "IncomeLossFromContinuingOperationsBeforeIncomeTaxesExtraordinaryItemsNoncontrollingInterest" },
                new string[] { "us-gaap:IncomeTaxExpenseBenefit", "IncomeTaxExpenseBenefit" },
                new string[] { "us-gaap:NetIncomeLoss", "NetIncomeLoss" },
                new string[] { "us-gaap:EarningsPerShareBasic", "EarningsPerShareBasic" },
                new string[] { "us-gaap:EarningsPerShareDiluted", "EarningsPerShareDiluted" },
                new string[] { "us-gaap:WeightedAverageNumberOfSharesOutstandingBasic", "WeightedAverageNumberOfSharesOutstandingBasic" },
                new string[] { "us-gaap:WeightedAverageNumberOfDilutedSharesOutstanding", "WeightedAverageNumberOfDilutedSharesOutstanding" },
                
            };

     

        string[][] statOfCompIncomeTags = {
                new string[] { "us-gaap:NetIncomeLoss", "NetIncomeLoss" },
                new string[] { "us-gaap:OtherComprehensiveIncomeLossForeignCurrencyTransactionAndTranslationAdjustmentNetOfTax", "OtherComprehensiveIncomeLossForeignCurrencyTransactionAndTranslationAdjustmentNetOfTax" },
                new string[] { "us-gaap:OtherComprehensiveIncomeUnrealizedGainLossOnDerivativesArisingDuringPeriodNetOfTax", "OtherComprehensiveIncomeUnrealizedGainLossOnDerivativesArisingDuringPeriodNetOfTax" },
                new string[] { "us-gaap:OtherComprehensiveIncomeLossReclassificationAdjustmentFromAOCIOnDerivativesNetOfTax", "OtherComprehensiveIncomeLossReclassificationAdjustmentFromAOCIOnDerivativesNetOfTax" },
                new string[] { "us-gaap:OtherComprehensiveIncomeLossDerivativesQualifyingAsHedgesNetOfTax", "OtherComprehensiveIncomeLossDerivativesQualifyingAsHedgesNetOfTax" },
                new string[] { "us-gaap:OtherComprehensiveIncomeUnrealizedHoldingGainLossOnSecuritiesArisingDuringPeriodNetOfTax", "OtherComprehensiveIncomeUnrealizedHoldingGainLossOnSecuritiesArisingDuringPeriodNetOfTax" },
                new string[] { "us-gaap:OtherComprehensiveIncomeLossReclassificationAdjustmentFromAOCIForSaleOfSecuritiesNetOfTax", "OtherComprehensiveIncomeLossReclassificationAdjustmentFromAOCIForSaleOfSecuritiesNetOfTax" },
                new string[] { "us-gaap:OtherComprehensiveIncomeLossAvailableForSaleSecuritiesAdjustmentNetOfTax", "OtherComprehensiveIncomeLossAvailableForSaleSecuritiesAdjustmentNetOfTax" },
                new string[] { "us-gaap:OtherComprehensiveIncomeLossNetOfTaxPortionAttributableToParent", "OtherComprehensiveIncomeLossNetOfTaxPortionAttributableToParent" },
                new string[] { "us-gaap:ComprehensiveIncomeNetOfTax", "ComprehensiveIncomeNetOfTax" }
            };


        string[][] stateOfCashFlowsTags =
        {
            new string[] { "us-gaap:DepreciationAmortizationAndAccretionNet", "DepreciationAmortizationAndAccretionNet" },
            new string[] { "us-gaap:ShareBasedCompensation", "ShareBasedCompensation" },
            new string[] { "us-gaap:DeferredIncomeTaxExpenseBenefit", "DeferredIncomeTaxExpenseBenefit" },
            new string[] { "us-gaap:OtherNoncashIncomeExpense", "OtherNoncashIncomeExpense" },
            new string[] { "us-gaap:IncreaseDecreaseInAccountsReceivable", "IncreaseDecreaseInAccountsReceivable" },
            new string[] { "us-gaap:IncreaseDecreaseInInventories", "IncreaseDecreaseInInventories" },
            new string[] { "us-gaap:IncreaseDecreaseInOtherReceivables", "IncreaseDecreaseInOtherReceivables" },
            new string[] { "us-gaap:IncreaseDecreaseInOtherOperatingAssets", "IncreaseDecreaseInOtherOperatingAssets" },
            new string[] { "us-gaap:IncreaseDecreaseInAccountsPayable", "IncreaseDecreaseInAccountsPayable" },
            new string[] { "us-gaap:IncreaseDecreaseInDeferredRevenue", "IncreaseDecreaseInDeferredRevenue" },
            new string[] { "us-gaap:IncreaseDecreaseInOtherOperatingLiabilities", "IncreaseDecreaseInOtherOperatingLiabilities" },
            new string[] { "us-gaap:NetCashProvidedByUsedInOperatingActivities", "NetCashProvidedByUsedInOperatingActivities" },
            new string[] { "us-gaap:PaymentsToAcquireAvailableForSaleSecurities", "PaymentsToAcquireAvailableForSaleSecurities" },
            new string[] { "us-gaap:ProceedsFromMaturitiesPrepaymentsAndCallsOfAvailableForSaleSecurities", "ProceedsFromMaturitiesPrepaymentsAndCallsOfAvailableForSaleSecurities" },
            new string[] { "us-gaap:ProceedsFromSaleOfAvailableForSaleSecurities", "ProceedsFromSaleOfAvailableForSaleSecurities" },
            new string[] { "us-gaap:PaymentsToAcquireBusinessesNetOfCashAcquired", "PaymentsToAcquireBusinessesNetOfCashAcquired" },
            new string[] { "us-gaap:PaymentsToAcquirePropertyPlantAndEquipment", "PaymentsToAcquirePropertyPlantAndEquipment" },
            new string[] { "us-gaap:PaymentsToAcquireIntangibleAssets", "PaymentsToAcquireIntangibleAssets" },
            new string[] { "us-gaap:PaymentsToAcquireOtherInvestments", "PaymentsToAcquireOtherInvestments" },
            new string[] { "us-gaap:PaymentsForProceedsFromOtherInvestingActivities", "PaymentsForProceedsFromOtherInvestingActivities" },
            new string[] { "us-gaap:NetCashProvidedByUsedInInvestingActivities", "NetCashProvidedByUsedInInvestingActivities" },
            new string[] { "us-gaap:ProceedsFromIssuanceOfCommonStock", "ProceedsFromIssuanceOfCommonStock" },
            new string[] { "us-gaap:ExcessTaxBenefitFromShareBasedCompensationFinancingActivities", "ExcessTaxBenefitFromShareBasedCompensationFinancingActivities" },
            new string[] { "us-gaap:PaymentsRelatedToTaxWithholdingForShareBasedCompensation", "PaymentsRelatedToTaxWithholdingForShareBasedCompensation" },
            new string[] { "us-gaap:PaymentsOfDividends", "PaymentsOfDividends" },
            new string[] { "us-gaap:PaymentsForRepurchaseOfCommonStock", "PaymentsForRepurchaseOfCommonStock" },
            new string[] { "us-gaap:ProceedsFromIssuanceOfLongTermDebt", "ProceedsFromIssuanceOfLongTermDebt" },
            new string[] { "us-gaap:RepaymentsOfLongTermDebt", "RepaymentsOfLongTermDebt" },
            new string[] { "us-gaap:ProceedsFromRepaymentsOfCommercialPaper", "ProceedsFromRepaymentsOfCommercialPaper" },
            new string[] { "us-gaap:NetCashProvidedByUsedInFinancingActivities", "NetCashProvidedByUsedInFinancingActivities" },
            new string[] { "us-gaap:CashAndCashEquivalentsPeriodIncreaseDecrease", "CashAndCashEquivalentsPeriodIncreaseDecrease" },
            new string[] { "us-gaap:CashAndCashEquivalentsAtCarryingValue", "CashAndCashEquivalentsAtCarryingValue" },
            new string[] { "us-gaap:IncomeTaxesPaidNet", "IncomeTaxesPaidNet" },
            new string[] { "us-gaap:InterestPaid", "InterestPaid" }

        };

        string[][] balanceSheetTags =
        {
            new string[] { "us-gaap:CashAndCashEquivalentsAtCarryingValue", "CashAndCashEquivalentsAtCarryingValue"},
            new string[] { "us-gaap:AvailableForSaleSecuritiesCurrent", "AvailableForSaleSecuritiesCurrent"},
            new string[] { "us-gaap:AccountsReceivableNetCurrent", "AccountsReceivableNetCurrent"},
            new string[] { "us-gaap:InventoryNet", "InventoryNet"},
            new string[] { "us-gaap:NontradeReceivablesCurrent", "NontradeReceivablesCurrent"},
            new string[] { "us-gaap:OtherAssetsCurrent", "OtherAssetsCurrent"},
            new string[] { "us-gaap:AssetsCurrent", "AssetsCurrent"},
            new string[] { "us-gaap:AvailableForSaleSecuritiesNoncurrent", "AvailableForSaleSecuritiesNoncurrent"},
            new string[] { "us-gaap:PropertyPlantAndEquipmentNet", "PropertyPlantAndEquipmentNet"},
            new string[] { "us-gaap:Goodwill", "Goodwill"},
            new string[] { "us-gaap:IntangibleAssetsNetExcludingGoodwill", "IntangibleAssetsNetExcludingGoodwill"},
            new string[] { "us-gaap:OtherAssetsNoncurrent", "OtherAssetsNoncurrent"},
            new string[] { "us-gaap:Assets", "Assets"},
            new string[] { "us-gaap:AccountsPayableCurrent", "AccountsPayableCurrent"},
            new string[] { "us-gaap:AccruedLiabilitiesCurrent", "AccruedLiabilitiesCurrent"},
            new string[] { "us-gaap:DeferredRevenueCurrent", "DeferredRevenueCurrent"},
            new string[] { "us-gaap:CommercialPaper", "CommercialPaper"},
            new string[] { "us-gaap:LongTermDebtCurrent", "LongTermDebtCurrent"},
            new string[] { "us-gaap:LiabilitiesCurrent", "LiabilitiesCurrent"},
            new string[] { "us-gaap:OperatingIncomeLoss", "OperatingIncomeLoss"},
            new string[] { "us-gaap:LongTermDebtNoncurrent", "LongTermDebtNoncurrent"},
            new string[] { "us-gaap:OtherLiabilitiesNoncurrent", "OtherLiabilitiesNoncurrent"},
            new string[] { "us-gaap:Liabilities", "Liabilities"},
            new string[] { "us-gaap:CommonStocksIncludingAdditionalPaidInCapital", "CommonStocksIncludingAdditionalPaidInCapital"},
            new string[] { "us-gaap:RetainedEarningsAccumulatedDeficit", "RetainedEarningsAccumulatedDeficit"},
            new string[] { "us-gaap:AccumulatedOtherComprehensiveIncomeLossNetOfTax", "AccumulatedOtherComprehensiveIncomeLossNetOfTax"},
            new string[] { "us-gaap:StockholdersEquity", "StockholdersEquity"},
            new string[] { "us-gaap:LiabilitiesAndStockholdersEquity", "LiabilitiesAndStockholdersEquity" }

        };

        string[][] epsTags =
        {
            new string[] { "us-gaap:WeightedAverageNumberOfSharesOutstandingBasic", "WeightedAverageNumberOfSharesOutstandingBasic" },
            new string[] { "us-gaap:WeightedAverageNumberDilutedSharesOutstandingAdjustment", "WeightedAverageNumberDilutedSharesOutstandingAdjustment" },
            new string[] { "us-gaap:WeightedAverageNumberOfDilutedSharesOutstanding", "WeightedAverageNumberOfDilutedSharesOutstanding" },
            new string[] { "us-gaap:EarningsPerShareBasic", "EarningsPerShareBasic" },
            new string[] { "us-gaap:EarningsPerShareDiluted", "EarningsPerShareDiluted" }

        };

        string[][] cashMarketSecTags =
        {

            // Cash - context: <<FI2017Q3>>_us-gaap_InvestmentTypeAxis_us-gaap_CashMember
            new string[] { "us-gaap:AvailableForSaleSecurities", "CashMarketSecCashAdjustCost", "_us-gaap_InvestmentTypeAxis_us-gaap_CashMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedGainBeforeTax", "CashMarketSecCashUnrealizedGains", "_us-gaap_InvestmentTypeAxis_us-gaap_CashMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedLossBeforeTax", "CashMarketSecCashUnrealizedLosses", "_us-gaap_InvestmentTypeAxis_us-gaap_CashMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAmortizedCost", "CashMarketSecCashCashAndEquivalents", "_us-gaap_InvestmentTypeAxis_us-gaap_CashMember" },
            new string[] { "us-gaap:CashAndCashEquivalentsAtCarryingValue", "CashMarketSecCashFairValue", "_us-gaap_InvestmentTypeAxis_us-gaap_CashMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesCurrent", "CashMarketSecCashCurrent", "_us-gaap_InvestmentTypeAxis_us-gaap_CashMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesNoncurrent", "CashMarketSecCashNoncurrent", "_us-gaap_InvestmentTypeAxis_us-gaap_CashMember" },

            // LEVEL 1
            // Money market funds
            new string[] { "us-gaap:AvailableForSaleSecurities", "CashMarketSecL1MoneyMktFundsAdjustCost", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member_us-gaap_InvestmentTypeAxis_us-gaap_MoneyMarketFundsMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedGainBeforeTax", "CashMarketSecL1MoneyMktFundsUnrealizedGains", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member_us-gaap_InvestmentTypeAxis_us-gaap_MoneyMarketFundsMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedLossBeforeTax", "CashMarketSecL1MoneyMktFundsUnrealizedLosses", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member_us-gaap_InvestmentTypeAxis_us-gaap_MoneyMarketFundsMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAmortizedCost", "CashMarketSecL1MoneyMktFundsCashAndEquivalents", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member_us-gaap_InvestmentTypeAxis_us-gaap_MoneyMarketFundsMember" },
            new string[] { "us-gaap:CashAndCashEquivalentsAtCarryingValue", "CashMarketSecL1MoneyMktFundsFairValue", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member_us-gaap_InvestmentTypeAxis_us-gaap_MoneyMarketFundsMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesCurrent", "CashMarketSecL1MoneyMktFundsCurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member_us-gaap_InvestmentTypeAxis_us-gaap_MoneyMarketFundsMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesNoncurrent", "CashMarketSecL1MoneyMktFundsNoncurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member_us-gaap_InvestmentTypeAxis_us-gaap_MoneyMarketFundsMember" },

            // Mutual funds
            new string[] { "us-gaap:AvailableForSaleSecurities", "CashMarketSecL1MutualFundsAdjustCost", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member_us-gaap_InvestmentTypeAxis_us-gaap_MutualFundMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedGainBeforeTax", "CashMarketSecL1MutualFundsUnrealizedGains", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member_us-gaap_InvestmentTypeAxis_us-gaap_MutualFundMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedLossBeforeTax", "CashMarketSecL1MutualFundsUnrealizedLosses", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member_us-gaap_InvestmentTypeAxis_us-gaap_MutualFundMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAmortizedCost", "CashMarketSecL1MutualFundsCashAndEquivalents", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member_us-gaap_InvestmentTypeAxis_us-gaap_MutualFundMember" },
            new string[] { "us-gaap:CashAndCashEquivalentsAtCarryingValue", "CashMarketSecL1MutualFundsFairValue", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member_us-gaap_InvestmentTypeAxis_us-gaap_MutualFundMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesCurrent", "CashMarketSecL1MutualFundsCurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member_us-gaap_InvestmentTypeAxis_us-gaap_MutualFundMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesNoncurrent", "CashMarketSecL1MutualFundsNoncurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member_us-gaap_InvestmentTypeAxis_us-gaap_MutualFundMember" },

            // Subtotal
            new string[] { "us-gaap:AvailableForSaleSecurities", "CashMarketSecL1SubtotalAdjustCost", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedGainBeforeTax", "CashMarketSecL1SubtotalUnrealizedGains", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedLossBeforeTax", "CashMarketSecL1SubtotalUnrealizedLosses", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAmortizedCost", "CashMarketSecL1SubtotalCashAndEquivalents", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member" },
            new string[] { "us-gaap:CashAndCashEquivalentsAtCarryingValue", "CashMarketSecL1SubtotalFairValue", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesCurrent", "CashMarketSecL1SubtotalCurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesNoncurrent", "CashMarketSecL1SubtotalNoncurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel1Member" },

            // LEVEL2
            // U.S. Treasury securities
            new string[] { "us-gaap:AvailableForSaleSecurities", "CashMarketSecL2USTresSecAdjustCost", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USTreasurySecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedGainBeforeTax", "CashMarketSecL2USTresSecUnrealizedGains", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USTreasurySecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedLossBeforeTax", "CashMarketSecL2USTresSecUnrealizedLosses", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USTreasurySecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAmortizedCost", "CashMarketSecL2USTresSecCashAndEquivalents", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USTreasurySecuritiesMember" },
            new string[] { "us-gaap:CashAndCashEquivalentsAtCarryingValue", "CashMarketSecL2USTresSecFairValue", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USTreasurySecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesCurrent", "CashMarketSecL2USTresSecCurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USTreasurySecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesNoncurrent", "CashMarketSecL2USTresSecNoncurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USTreasurySecuritiesMember" },

            // U.S. agency securities
            new string[] { "us-gaap:AvailableForSaleSecurities", "CashMarketSecL2GovAgenciesSecAdjustCost", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USGovernmentAgenciesDebtSecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedGainBeforeTax", "CashMarketSecL2GovAgenciesSecUnrealizedGains", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USGovernmentAgenciesDebtSecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedLossBeforeTax", "CashMarketSecL2GovAgenciesSecUnrealizedLosses", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USGovernmentAgenciesDebtSecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAmortizedCost", "CashMarketSecL2GovAgenciesSecCashAndEquivalents", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USGovernmentAgenciesDebtSecuritiesMember" },
            new string[] { "us-gaap:CashAndCashEquivalentsAtCarryingValue", "CashMarketSecL2GovAgenciesSecFairValue", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USGovernmentAgenciesDebtSecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesCurrent", "CashMarketSecL2GovAgenciesSecCurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USGovernmentAgenciesDebtSecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesNoncurrent", "CashMarketSecL2GovAgenciesSecNoncurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USGovernmentAgenciesDebtSecuritiesMember" },

            // Non-U.S. government securities
            new string[] { "us-gaap:AvailableForSaleSecurities", "CashMarketSecL2NonUSGovSecAdjustCost", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_ForeignGovernmentDebtSecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedGainBeforeTax", "CashMarketSecL2NonUSGovSecUnrealizedGains", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_ForeignGovernmentDebtSecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedLossBeforeTax", "CashMarketSecL2NonUSGovSecUnrealizedLosses", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_ForeignGovernmentDebtSecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAmortizedCost", "CashMarketSecL2NonUSGovSecCashAndEquivalents", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_ForeignGovernmentDebtSecuritiesMember" },
            new string[] { "us-gaap:CashAndCashEquivalentsAtCarryingValue", "CashMarketSecL2NonUSGovSecFairValue", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_ForeignGovernmentDebtSecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesCurrent", "CashMarketSecL2NonUSGovSecCurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_ForeignGovernmentDebtSecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesNoncurrent", "CashMarketSecL2NonUSGovSecNoncurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_ForeignGovernmentDebtSecuritiesMember" },

            // Certificates of deposit and time deposits
            new string[] { "us-gaap:AvailableForSaleSecurities", "CashMarketSecL2BankTimeDepositsAdjustCost", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_BankTimeDepositsMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedGainBeforeTax", "CashMarketSecL2BankTimeDepositsUnrealizedGains", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_BankTimeDepositsMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedLossBeforeTax", "CashMarketSecL2BankTimeDepositsUnrealizedLosses", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_BankTimeDepositsMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAmortizedCost", "CashMarketSecL2BankTimeDepositsCashAndEquivalents", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_BankTimeDepositsMember" },
            new string[] { "us-gaap:CashAndCashEquivalentsAtCarryingValue", "CashMarketSecL2BankTimeDepositsFairValue", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_BankTimeDepositsMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesCurrent", "CashMarketSecL2BankTimeDepositsCurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_BankTimeDepositsMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesNoncurrent", "CashMarketSecL2BankTimeDepositsNoncurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_BankTimeDepositsMember" },

            // Commercial paper
            new string[] { "us-gaap:AvailableForSaleSecurities", "CashMarketSecL2CommercialPaperAdjustCost", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_CommercialPaperMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedGainBeforeTax", "CashMarketSecL2CommercialPaperUnrealizedGains", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_CommercialPaperMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedLossBeforeTax", "CashMarketSecL2CommercialPaperUnrealizedLosses", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_CommercialPaperMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAmortizedCost", "CashMarketSecL2CommercialPaperCashAndEquivalents", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_CommercialPaperMember" },
            new string[] { "us-gaap:CashAndCashEquivalentsAtCarryingValue", "CashMarketSecL2CommercialPaperFairValue", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_CommercialPaperMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesCurrent", "CashMarketSecL2CommercialPaperCurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_CommercialPaperMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesNoncurrent", "CashMarketSecL2CommercialPaperNoncurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_CommercialPaperMember" },

            // Corporate securities
            new string[] { "us-gaap:AvailableForSaleSecurities", "CashMarketSecL2CorpDeptAdjustCost", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_CorporateDebtSecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedGainBeforeTax", "CashMarketSecL2CorpDeptUnrealizedGains", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_CorporateDebtSecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedLossBeforeTax", "CashMarketSecL2CorpDeptUnrealizedLosses", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_CorporateDebtSecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAmortizedCost", "CashMarketSecL2CorpDeptCashAndEquivalents", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_CorporateDebtSecuritiesMember" },
            new string[] { "us-gaap:CashAndCashEquivalentsAtCarryingValue", "CashMarketSecL2CorpDeptFairValue", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_CorporateDebtSecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesCurrent", "CashMarketSecL2CorpDeptCurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_CorporateDebtSecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesNoncurrent", "CashMarketSecL2CorpDeptNoncurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_CorporateDebtSecuritiesMember" },

            // Municipal securities
            new string[] { "us-gaap:AvailableForSaleSecurities", "CashMarketSecL2USStatesAndPoliticalSubdivAdjustCost", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USStatesAndPoliticalSubdivisionsMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedGainBeforeTax", "CashMarketSecL2USStatesAndPoliticalSubdivUnrealizedGains", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USStatesAndPoliticalSubdivisionsMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedLossBeforeTax", "CashMarketSecL2USStatesAndPoliticalSubdivUnrealizedLosses", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USStatesAndPoliticalSubdivisionsMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAmortizedCost", "CashMarketSecL2USStatesAndPoliticalSubdivCashAndEquivalents", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USStatesAndPoliticalSubdivisionsMember" },
            new string[] { "us-gaap:CashAndCashEquivalentsAtCarryingValue", "CashMarketSecL2USStatesAndPoliticalSubdivFairValue", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USStatesAndPoliticalSubdivisionsMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesCurrent", "CashMarketSecL2USStatesAndPoliticalSubdivCurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USStatesAndPoliticalSubdivisionsMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesNoncurrent", "CashMarketSecL2USStatesAndPoliticalSubdivNoncurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_USStatesAndPoliticalSubdivisionsMember" },

            // Mortgage- and asset-backed securities
            new string[] { "us-gaap:AvailableForSaleSecurities", "CashMarketSecL2AssetBackedSecAdjustCost", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_AssetBackedSecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedGainBeforeTax", "CashMarketSecL2AssetBackedSecUnrealizedGains", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_AssetBackedSecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedLossBeforeTax", "CashMarketSecL2AssetBackedSecUnrealizedLosses", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_AssetBackedSecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAmortizedCost", "CashMarketSecL2AssetBackedSecCashAndEquivalents", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_AssetBackedSecuritiesMember" },
            new string[] { "us-gaap:CashAndCashEquivalentsAtCarryingValue", "CashMarketSecL2AssetBackedSecFairValue", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_AssetBackedSecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesCurrent", "CashMarketSecL2AssetBackedSecCurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_AssetBackedSecuritiesMember" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesNoncurrent", "CashMarketSecL2AssetBackedSecNoncurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_InvestmentTypeAxis_us-gaap_AssetBackedSecuritiesMember" },

            // Subtotal
            new string[] { "us-gaap:AvailableForSaleSecurities", "CashMarketSecL2SubtotalAdjustCost", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedGainBeforeTax", "CashMarketSecL2SubtotalUnrealizedGains", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedLossBeforeTax", "CashMarketSecL2SubtotalUnrealizedLosses", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAmortizedCost", "CashMarketSecL2SubtotalCashAndEquivalents", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member" },
            new string[] { "us-gaap:CashAndCashEquivalentsAtCarryingValue", "CashMarketSecL2SubtotalFairValue", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesCurrent", "CashMarketSecL2SubtotalCurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesNoncurrent", "CashMarketSecL2SubtotalNoncurrent", "_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member" },

            // Total
            new string[] { "us-gaap:AvailableForSaleSecurities", "CashMarketSecAdjustCost" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedGainBeforeTax", "CashMarketSecUnrealizedGains" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAccumulatedGrossUnrealizedLossBeforeTax", "CashMarketSecUnrealizedLosses" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesAmortizedCost", "CashMarketSecCashAndEquivalents" },
            new string[] { "us-gaap:CashAndCashEquivalentsAtCarryingValue", "CashMarketSecFairValue" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesCurrent", "CashMarketSecCurrent" },
            new string[] { "us-gaap:AvailableForSaleSecuritiesNoncurrent", "CashMarketSecNoncurrent" },
        };

        string[][] derivateFinInstrAssetsLiabilitiesTags =
        {
            // Derivative assets (1)
            // Foreign exchange contracts
            new string[] { "us-gaap:DerivativeFairValueOfDerivativeAsset", "DerivAssetsFXContractsFairValueDesignated", "_us-gaap_BalanceSheetLocationAxis_us-gaap_OtherCurrentAssetsMember_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_ForeignExchangeContractMember_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_HedgingDesignationAxis_us-gaap_DesignatedAsHedgingInstrumentMember" },
            new string[] { "us-gaap:DerivativeFairValueOfDerivativeAsset", "DerivAssetsFXContractsFairValueNondesignated", "_us-gaap_BalanceSheetLocationAxis_us-gaap_OtherCurrentAssetsMember_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_ForeignExchangeContractMember_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_HedgingDesignationAxis_us-gaap_NondesignatedMember" },
            new string[] { "us-gaap:DerivativeFairValueOfDerivativeAsset", "DerivAssetsFXContractsFairValue", "_us-gaap_BalanceSheetLocationAxis_us-gaap_OtherCurrentAssetsMember_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_ForeignExchangeContractMember_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member" },

            // Interest rate contracts
            new string[] { "us-gaap:DerivativeFairValueOfDerivativeAsset", "DerivAssetsIntRatesContractsFairValueDesignated", "_us-gaap_BalanceSheetLocationAxis_us-gaap_OtherCurrentAssetsMember_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_InterestRateContractMember_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_HedgingDesignationAxis_us-gaap_DesignatedAsHedgingInstrumentMember" },
            new string[] { "us-gaap:DerivativeFairValueOfDerivativeAsset", "DerivAssetsIntRatesContractsFairValueNondesignated", "_us-gaap_BalanceSheetLocationAxis_us-gaap_OtherCurrentAssetsMember_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_InterestRateContractMember_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_HedgingDesignationAxis_us-gaap_NondesignatedMember" },
            new string[] { "us-gaap:DerivativeFairValueOfDerivativeAsset", "DerivAssetsIntRatesContractsFairValue", "_us-gaap_BalanceSheetLocationAxis_us-gaap_OtherCurrentAssetsMember_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_InterestRateContractMember_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member" },

            // Derivative liabilities
            // Foreign exchange contracts
            new string[] { "us-gaap:DerivativeFairValueOfDerivativeLiability", "DerivLiabilityFXContractsFairValueDesignated", "_us-gaap_BalanceSheetLocationAxis_us-gaap_AccruedLiabilitiesMember_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_ForeignExchangeContractMember_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_HedgingDesignationAxis_us-gaap_DesignatedAsHedgingInstrumentMember" },
            new string[] { "us-gaap:DerivativeFairValueOfDerivativeLiability", "DerivLiabilityFXContractsFairValueNondesignated", "_us-gaap_BalanceSheetLocationAxis_us-gaap_AccruedLiabilitiesMember_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_ForeignExchangeContractMember_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_HedgingDesignationAxis_us-gaap_NondesignatedMember" },
            new string[] { "us-gaap:DerivativeFairValueOfDerivativeLiability", "DerivLiabilityFXContractsFairValue", "_us-gaap_BalanceSheetLocationAxis_us-gaap_AccruedLiabilitiesMember_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_ForeignExchangeContractMember_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member" },

            // Interest rate contracts
                                                                                                                                     
            new string[] { "us-gaap:DerivativeFairValueOfDerivativeLiability", "DerivLiabilityIntRatesContractsFairValueDesignated", "_us-gaap_BalanceSheetLocationAxis_us-gaap_AccruedLiabilitiesMember_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_InterestRateContractMember_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_HedgingDesignationAxis_us-gaap_DesignatedAsHedgingInstrumentMember" },
            new string[] { "us-gaap:DerivativeFairValueOfDerivativeLiability", "DerivLiabilityIntRatesContractsFairValueNondesignated", "_us-gaap_BalanceSheetLocationAxis_us-gaap_AccruedLiabilitiesMember_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_InterestRateContractMember_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member_us-gaap_HedgingDesignationAxis_us-gaap_NondesignatedMember" },
            new string[] { "us-gaap:DerivativeFairValueOfDerivativeLiability", "DerivLiabilityIntRatesContractsFairValue", "_us-gaap_BalanceSheetLocationAxis_us-gaap_AccruedLiabilitiesMember_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_InterestRateContractMember_us-gaap_FairValueByFairValueHierarchyLevelAxis_us-gaap_FairValueInputsLevel2Member" },

        };

        string[][] instrumentsGainLossTags =
        {
            // Gains/(Losses) recognized in OCI – effective portion
            new string[] { "us-gaap:DerivativeInstrumentsGainLossRecognizedInOtherComprehensiveIncomeEffectivePortionNet", "DerivativesGLOCICFHedgesFXContracts", "_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_ForeignExchangeContractMember_us-gaap_DerivativeInstrumentsGainLossByHedgingRelationshipAxis_us-gaap_CashFlowHedgingMember" },
            new string[] { "us-gaap:DerivativeInstrumentsGainLossRecognizedInOtherComprehensiveIncomeEffectivePortionNet", "DerivativesGLOCICFHedgesIntRatesContracts", "_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_InterestRateContractMember_us-gaap_DerivativeInstrumentsGainLossByHedgingRelationshipAxis_us-gaap_CashFlowHedgingMember" },
            new string[] { "us-gaap:DerivativeInstrumentsGainLossRecognizedInOtherComprehensiveIncomeEffectivePortionNet", "DerivativesGLOCICFHedges", "_us-gaap_DerivativeInstrumentsGainLossByHedgingRelationshipAxis_us-gaap_CashFlowHedgingMember" },
            new string[] { "us-gaap:DerivativeInstrumentsGainLossRecognizedInOtherComprehensiveIncomeEffectivePortionNet", "DerivativesGLOCINetInvestmentsFXDebt", "_us-gaap_DerivativeInstrumentRiskAxis_aapl_ForeignCurrencyDebtMember_us-gaap_DerivativeInstrumentsGainLossByHedgingRelationshipAxis_us-gaap_NetInvestmentHedgingMember" },

            // Gains/(Losses) reclassified from AOCI into net income – effective portion
            new string[] { "us-gaap:DerivativeInstrumentsGainLossReclassifiedFromAccumulatedOCIIntoIncomeEffectivePortionNet", "DerivativesGLAOCICFHedgesFXContracts", "_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_ForeignExchangeContractMember_us-gaap_DerivativeInstrumentsGainLossByHedgingRelationshipAxis_us-gaap_CashFlowHedgingMember" },
            new string[] { "us-gaap:DerivativeInstrumentsGainLossReclassifiedFromAccumulatedOCIIntoIncomeEffectivePortionNet", "DerivativesGLAOCICFHedgesIntRatesContracts", "_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_InterestRateContractMember_us-gaap_DerivativeInstrumentsGainLossByHedgingRelationshipAxis_us-gaap_CashFlowHedgingMember" },
            new string[] { "us-gaap:DerivativeInstrumentsGainLossReclassifiedFromAccumulatedOCIIntoIncomeEffectivePortionNet", "DerivativesGLAOCICFHedges", "_us-gaap_DerivativeInstrumentsGainLossByHedgingRelationshipAxis_us-gaap_CashFlowHedgingMember" },

            // Gains/(Losses) on derivative instruments
            new string[] { "us-gaap:DerivativeGainLossOnDerivativeNet", "DerivativesGLFairValueHedgeIntRates", "_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_InterestRateContractMember_us-gaap_DerivativeInstrumentsGainLossByHedgingRelationshipAxis_us-gaap_FairValueHedgingMember" },

            // Gains/(Losses) related to hedged items
            new string[] { "us-gaap:ChangeInUnrealizedGainLossOnHedgedItemInFairValueHedge1", "DerivativesGLFairValueHedgeFixRateDebt", "_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_InterestRateContractMember_us-gaap_DerivativeInstrumentsGainLossByHedgingRelationshipAxis_us-gaap_FairValueHedgingMember" }
        };

        string[][] derivativeNotionalAmountTags =
        {
            // Instruments designated as accounting hedges
            new string[] { "invest:DerivativeNotionalAmount", "DerivativeNotionalAmountFXNotionalAsHendgingInstrument", "_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_ForeignExchangeContractMember_us-gaap_HedgingDesignationAxis_us-gaap_DesignatedAsHedgingInstrumentMember"},
            new string[] { "invest:DerivativeNotionalAmount", "DerivativeNotionalAmountIntRatesNotionalAsHendgingInstrument", "_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_InterestRateContractMember_us-gaap_HedgingDesignationAxis_us-gaap_DesignatedAsHedgingInstrumentMember"},
            new string[] { "aapl:DerivativeCounterpartyCreditRiskExposure", "DerivativeNotionalAmountFXCreditRiskAsHendgingInstrument", "_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_ForeignExchangeContractMember_us-gaap_HedgingDesignationAxis_us-gaap_DesignatedAsHedgingInstrumentMember"},
            new string[] { "aapl:DerivativeCounterpartyCreditRiskExposure", "DerivativeNotionalAmountIntRatesCreditRiskAsHendgingInstrument", "_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_InterestRateContractMember_us-gaap_HedgingDesignationAxis_us-gaap_DesignatedAsHedgingInstrumentMember"},

            // Instruments not designated as accounting hedges
            new string[] { "invest:DerivativeNotionalAmount", "DerivativeNotionalAmountFXNotionalNonhedging", "_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_ForeignExchangeContractMember_us-gaap_HedgingDesignationAxis_us-gaap_NondesignatedMember" },
            new string[] { "aapl:DerivativeCounterpartyCreditRiskExposure", "DerivativeNotionalAmountFXCreditRiskNonhedging", "_us-gaap_DerivativeInstrumentRiskAxis_us-gaap_ForeignExchangeContractMember_us-gaap_HedgingDesignationAxis_us-gaap_NondesignatedMember" },
        };

        // TODO: need to add tags related to debt etc.

        string[][] reportableOperatingsTags =
        {
            // Americas
            new string[] { "us-gaap:SalesRevenueNet", "RepOperatingAmericasNetSales", "_us-gaap_StatementBusinessSegmentsAxis_aapl_AmericasSegmentMember"  },
            new string[] { "us-gaap:OperatingIncomeLoss", "RepOperatingAmericasOperatingIncome", "_us-gaap_StatementBusinessSegmentsAxis_aapl_AmericasSegmentMember"  },

            // Europe
            new string[] { "us-gaap:SalesRevenueNet", "RepOperatingEuropeNetSales", "_us-gaap_StatementBusinessSegmentsAxis_aapl_EuropeSegmentMember"  },
            new string[] { "us-gaap:OperatingIncomeLoss", "RepOperatingEuropeOperatingIncome", "_us-gaap_StatementBusinessSegmentsAxis_aapl_EuropeSegmentMember"  },

            // Greater China
            new string[] { "us-gaap:SalesRevenueNet", "RepOperatingChinaNetSales", "_us-gaap_StatementBusinessSegmentsAxis_aapl_GreaterChinaSegmentMember"  },
            new string[] { "us-gaap:OperatingIncomeLoss", "RepOperatingChinaOperatingIncome", "_us-gaap_StatementBusinessSegmentsAxis_aapl_GreaterChinaSegmentMember"  },

            // Japan
            new string[] { "us-gaap:SalesRevenueNet", "RepOperatingJapanNetSales", "_us-gaap_StatementBusinessSegmentsAxis_aapl_JapanSegmentMember"  },
            new string[] { "us-gaap:OperatingIncomeLoss", "RepOperatingJapanOperatingIncome", "_us-gaap_StatementBusinessSegmentsAxis_aapl_JapanSegmentMember"  },

            // Rest of Asia Pacific
            new string[] { "us-gaap:SalesRevenueNet", "RepOperatingAsiaPacificNetSales", "_us-gaap_StatementBusinessSegmentsAxis_aapl_RestOfAsiaPacificSegmentMember"  },
            new string[] { "us-gaap:OperatingIncomeLoss", "RepOperatingAsiaPacificOperatingIncome", "_us-gaap_StatementBusinessSegmentsAxis_aapl_RestOfAsiaPacificSegmentMember" }

        };

        string[][] commonStockDividentsTags =
        {
            new string[] { "us-gaap:CommonStockDividendsPerShareDeclared", "CommonStockDividendsPerShareDeclared" },
            new string[] { "us-gaap:PaymentsOfDividendsCommonStock", "PaymentsOfDividendsCommonStock" }
        };
        #endregion

        

        protected override XmlNamespaceManager PrepareNamespaceMngr(XmlDocument doc)
        {

            XmlNamespaceManager nsmgr = base.PrepareNamespaceMngr(doc);
            nsmgr.AddNamespace("us-gaap",   "http://fasb.org/us-gaap/2017-01-31");
            
            return nsmgr;
        }
    }
}
