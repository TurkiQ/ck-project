﻿using System;

namespace ck_project.Helpers
{
    public class InstallationCalculationHelper
    {
        GeneralHelper helper = new GeneralHelper();
        public double CalculateMaterialRetailPrice(lead lead)
        {
            double materialRetailCost = 0;
            double materialRate = 0;
            foreach (var item in lead.installations)
            {
                if (item.ov_material_rate == null)
                {
                    materialRate = helper.GetApplicableRate("Material Cost Rate");
                }
                else
                {
                    materialRate = (double)item.ov_material_rate;
                }
               
                foreach (var task in item.tasks_installation)
                {
                    materialRetailCost += task.m_cost;
                }
            }

            if (materialRate == 0)
            {
                return materialRetailCost;
            }
            else
            {
                return materialRetailCost * materialRate;
            }
                
        }

        public double CalculateEstimatedHours(lead lead)
        {
            double hours = 0;
            foreach (var item in lead.installations)
            {
                if (item.tasks_installation != null)
                {
                    foreach (var task in item.tasks_installation)
                    {
                        hours += task.hours;
                    }
                }
            }

            return hours;
        }

        public double CalculateBillableHours(double estimatedHours)
        {
            return (Math.Ceiling(estimatedHours / 8.0)) * 8.0;
        }

        public double CalculateInstallationDays(double billablehours)
        {
            return billablehours / 8;
        }

        public double CalculateTileInstallationDays(double totalTilePrice)
        {
            double rate = helper.GetApplicableRate(Constants.rate_Name_Hourly_Tile_Installer);
            if (rate != 0)
            {
                return Math.Ceiling(totalTilePrice / rate / 8);
            }
            return 0;
        }

        public double CalculateTotalInstallationDays(double? installationdays, double? tileInstallationDays) 
        {
            double totalInstallDays = 0;
            if (tileInstallationDays != null)
            {
                totalInstallDays += (double)tileInstallationDays;
            }

            if (installationdays != null)
            {
                totalInstallDays += (double)installationdays;
            }
            return totalInstallDays;
        }

        public double CalculateNumberOfHotelNights(double installationdays, string recommendation)
        {
            return recommendation != null && recommendation.Equals(Constants.install_recommendation_hotel) ? installationdays - 1 : 0;
        }

        public double CalculateTravelTimeOneWay(double oneWayMileage)
        {
            return Math.Round(oneWayMileage * 1.35 / 60, 2);
        }

        public double CalculatePaidTravelTimeOneWay(double? traveltimeOneway)
        {
            return traveltimeOneway != null && (double) traveltimeOneway >= 1 ? (double)traveltimeOneway - 1 : 0;
        }

        public double CalculateBothWayMilesToJob(double oneWayMileage)
        {
            return (double)oneWayMileage * 2;
        }

        public double CalculateHotelRoundTrip(double installationdays)
        {
            return (Math.Ceiling((installationdays/5) / 1.0)) * 1.0;
        }

        public double CalculateTotalMiles(double installationdays, string recommendation, double oneWayMile)
        {
            if (recommendation != null && recommendation.Equals(Constants.install_recommendation_hotel))
            {
                return Math.Round(this.CalculateHotelRoundTrip(installationdays) * this.CalculateBothWayMilesToJob(oneWayMile), 2);
            }

            return Math.Round(installationdays * this.CalculateBothWayMilesToJob(oneWayMile), 2);
        }

        public double CalculateTotalApplicableTravelHours(double? installationDays, double? traveltimeOneway, string recommendation)
        {
            if (recommendation != null && installationDays != null)
            {
                if (recommendation.Equals(Constants.install_recommendation_hotel))
                {
                    return Math.Round(this.CalculateHotelRoundTrip((double)installationDays) * this.CalculatePaidTravelTimeOneWay(traveltimeOneway) * 2, 2);
                }
                else
                {
                    return Math.Round((double)installationDays * this.CalculatePaidTravelTimeOneWay(traveltimeOneway) * 2, 2);
                }
            }

            return 0;
        }

        public double CalculatePerDiem(double installationDays, string recommendation)
        {
            if (recommendation != null && recommendation.Equals(Constants.install_recommendation_hotel))
            {
                return Math.Round(installationDays * 2 * helper.GetApplicableRate(Constants.rate_Name_Per_Diem), 2);
            }

            return 0;
        }

        public double CalculateLaborOnlyExpense(double billableHours)
        {
            return Math.Round(billableHours * (helper.GetApplicableRate(Constants.rate_Name_Hourly_Lead_Installer) + helper.GetApplicableRate(Constants.rate_Name_Hourly_Junior_Installer)), 2);
        }

        public double CalculateTravelExpense(double installationsDays, double? traveltimeOneway, string recommendation)
        {
            return Math.Round(this.CalculateTotalApplicableTravelHours(installationsDays, traveltimeOneway, recommendation) * helper.GetApplicableRate(Constants.rate_Name_Travel), 2);
        }

        public double CalculateMileageExpense(double totalMiles, string recommendation)
        {
            if (recommendation != null && recommendation.Equals(Constants.install_recommendation_hotel))
            {
                return totalMiles * helper.GetApplicableRate(Constants.rate_Name_Mileage);
            }

            return Math.Round(totalMiles, 2);
        }

        public double CalculateHotelExpense(double numberOfHotelNights, string recommendation)
        {
            if (recommendation != null && recommendation.Equals(Constants.install_recommendation_hotel))
            {
                return Math.Round(numberOfHotelNights * helper.GetApplicableRate(Constants.rate_Name_Hotel), 2);
            }

            return 0;
        }

        // include labor cost, travel cost, mileage expense, hotel expense, building permit cost, operational cost and per diem cost
        public double CalculateTotalLaborExpense(lead lead)
        {
            double totalLaborCost = 0.00;
            if (lead.installations != null)
            {
                foreach (var item in lead.installations)
                {
                    if (item.installation_labor_only_cost != null)
                    {
                        totalLaborCost += (double)item.installation_labor_only_cost;
                    }

                    if (item.mileage_expense != null)
                    {
                        totalLaborCost += (double)item.total_travel_cost;
                    }

                    if (item.mileage_expense != null)
                    {
                        totalLaborCost += (double)item.mileage_expense;
                    }

                    if (item.hotel_expense != null)
                    {
                        totalLaborCost += (double)item.hotel_expense;
                    }

                    if (item.building_permit_cost != null)
                    {
                        totalLaborCost += (double)item.building_permit_cost;
                    }

                    if (item.total_operational_expenses != null)
                    {
                        totalLaborCost += (double)item.total_operational_expenses;
                    }

                    if (item.total_per_diem_cost != null)
                    {
                        totalLaborCost += (double)item.total_per_diem_cost;
                    }
                }
            }
            return Math.Round(totalLaborCost, 2);
        }

        // only need buildling permit if the project is installed and jobsite is in the city
        public string GetRecommendation(double onewaymile) {
            return onewaymile > 89 ? "Hotel" : "Travel";
        }

        public double CalculateBuildingPermit(lead lead)
        {
            double buildingPermitCost = 0.00;
            if (lead.in_city && lead.delivery_status.delivery_status_name.Equals(Constants.deliver_Status_Installed))
            {
                if (lead.installations != null)
                {
                    foreach (var item in lead.installations)
                    {
                        if (item.installation_labor_only_cost != null)
                        {
                            buildingPermitCost += (double)item.installation_labor_only_cost;
                        }

                        if (item.total_construction_materials_cost != null)
                        {
                            buildingPermitCost += (double)item.total_construction_materials_cost;
                        }
                    }

                    buildingPermitCost += new TotalCostHelper().CalculateProductCost(lead);
                    return Math.Round(helper.GetBuildingPermitAmount(buildingPermitCost), 2);
                }
            }

            return buildingPermitCost;
        }
    }
}