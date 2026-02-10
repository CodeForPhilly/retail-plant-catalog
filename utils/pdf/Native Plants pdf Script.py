# # Native Plant pdf Update Script

# ## Import Statements

import pandas as pd
import re
import pdfplumber
import requests
import itertools
import warnings


pdf_path = "ERA-Vendor-Table-FINAL-low-res-2kfekyd.pdf"
warnings.filterwarnings("ignore", module="pdfplumber")
    
#Use pdfplumber to extract text from all pages
all_text=""
with pdfplumber.open(pdf_path) as pdf:
    for page in pdf.pages[:200]:
        all_text += page.extract_text() + "\n"

# Extract structured rows using a regular pattern

lines = all_text.splitlines()       
structured_rows = []

# Skip the header and explanatory text

for line in lines:
    match = re.match(r"^([A-Z][a-z]+ [a-z]+)\s+([A-Z]+)\s+(.+?)\s+([A-Za-z\s]+)\s+([A-Z]{2})\s+(https?://\S+)", line)
    if match:
        structured_rows.append({
            "Scientific Name": match.group(1),
            "USDA Symbol": match.group(2),
            "Company": match.group(3),
            "City": match.group(4).strip(),
            "State": match.group(5),
            "Website": match.group(6)
        })



#Extract unique scientific names to use in API calls
unique_scientific_names = sorted(set(row["Scientific Name"] for row in structured_rows))

# Show a sample list of names for query
unique_scientific_names[:100]


# API Query Functions
API_KEY = "" #add API Key
BASE_URL = "http://app.plantagents.org"


headers ={
"Authorization": f"Bearer {API_KEY}"
}

def find_plant_by_name(plant_name):
    url = f"{BASE_URL}/Plant/FindByName"
    params = {"plantName": plant_name}
    response = requests.get(url, headers=headers,params=params)
    print("Request URL:", response.url)
    print("Status Code:", response.status_code)
    if response.status_code == 401:
        print("X Authorization error: Check your token format.")
    response.raise_for_status()
    return response.json()


def find_vendors_for_plant(plant_name, zip_code="32806", radius=500, limit=50):
    url = f"{BASE_URL}/Plant/FindVendorsForPlantName"

    params = {
        "plantName": plant_name,
        "zipCode": zip_code,
        "radius": radius,
        "limit": limit
    }

    try:
        response = requests.get(url, headers=headers, params=params)
        print("Vendor Request URL:", response.url)
        print("Status Code:", response.status_code)
        print("Response Text:", response.text[:100])
        response.raise_for_status()
        return response.json()
    except requests.exceptions.HTTPError as e:
        print(f"Vendor error for '{plant_name}':", e)
        return[]
    
    

# Get the common names
plant_data = []
vendor_data = []

for name in unique_scientific_names:
    plant_resp = find_plant_by_name(name)
    if not plant_resp:
        continue

    plant = plant_resp[0]
    common_name = plant.get("commonName")
    symbol = plant.get("symbol")
    plant_id = plant.get("id")

    vendors = find_vendors_for_plant(name)
    for v in vendors:
        vendor_data.append({
            "Scientific Name": name,
            "Common Name": common_name,
            "USDA Symbol": symbol,
            "Company": v.get("storeName"),
            "Address": v.get("address"),
            "State": v.get("state"),
            "Website": v.get("storeUrl"),
            "All Native?": v.get("allNative")
        })

# Convert to DataFrame
df = pd.DataFrame(vendor_data)
df.to_csv("Updated_vendor_list.csv", index=False)