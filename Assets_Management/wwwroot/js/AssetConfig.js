window.assetConfigs = {
	'C': {
		specificFields: [
			{ id: 'processor', label: 'Processor', type: 'text' },
			{ id: 'ram_size', label: 'RAM', type: 'text' },
			{ id: 'storage', label: 'Storage', type: 'text' },
			{ id: 'HddType', label: 'HDD Type', type: 'select', options: ['SATA', 'NVMe', 'NORMAL', 'HDD', 'SSD', 'NVMe+SSD', 'NVME+HDD',] },
			{ id: 'Software', label: 'Software', type: 'text' },
			{ id: 'SoftwareVersion', label: 'Software Version', type: 'text' },
			{ id: 'SoftwareKey', label: 'Software Key', type: 'text' },
			{ id: 'SoftwareLicence', label: 'Software Licence', type: 'text' }, 
		],
		showNetwork: true,
		showWarranty: true
	},

	'M': {
		specificFields: [
			{ id: 'screen_size', label: 'Screen Size', type: 'text', required: true },
			{ id: 'resolution', label: 'Resolution', type: 'text' },
			{ id: 'panel_type', label: 'Panel Type', type: 'text' }
		],
		showNetwork: false,
		showWarranty: true
	},

	'L': {
		specificFields: [
			{ id: 'processor', label: 'Processor', type: 'text' },
			{ id: 'ram_size', label: 'RAM', type: 'text' },
			{ id: 'storage', label: 'HDD', type: 'text' },
			{ id: 'HddType', label: 'HDD Type', type: 'select', options: ['SATA', 'NVMe', 'NORMAL', 'HDD', 'SSD', 'NVMe+SSD', 'NVME+HDD',] },
			{ id: 'screen_size', label: 'Screen Size', type: 'text', required: true },
			{ id: 'resolution', label: 'Resolution', type: 'text' },
			{ id: 'panel_type', label: 'Panel Type', type: 'text' },
			{ id: 'Generation', label: 'Generation', type: 'text' },
			{ id: 'BattrySrNo', label: 'Battry Sr No', type: 'text' },
			{ id: 'AdapterSRNO', label: 'Adapter Sr No', type: 'text' },
			{ id: 'Product_Name', label: 'Product Name', type: 'text' },
			{ id: 'ServiceTag', label: 'Service Tag', type: 'text' },
			{ id: 'WIFIMacID', label: 'WIFI MacId/LAN MacId/Bluetooth', type: 'text' },
			{ id: 'Software', label: 'Software', type: 'text' },
			{ id: 'SoftwareVersion', label: 'Software Version', type: 'text' },
			{ id: 'SoftwareKey', label: 'Software Key', type: 'text' },
			{ id: 'SoftwareLicence', label: 'Software Licence', type: 'text' },
		],
		showNetwork: true,
		showWarranty: true
	},

	'T': {
		specificFields: [
			{ id: 'processor', label: 'Processor', type: 'text' },
			{ id: 'ram_size', label: 'RAM', type: 'text' },
			{ id: 'storage', label: 'Storage', type: 'text' },
			{ id: 'screen_size', label: 'Screen Size', type: 'text' },
			{ id: 'Software', label: 'Software', type: 'text' },
			{ id: 'SoftwareVersion', label: 'Software Version', type: 'text' },
			{ id: 'SoftwareKey', label: 'Software Key', type: 'text' },
			{ id: 'SoftwareLicence', label: 'Software Licence', type: 'text' },
		],
		showNetwork: true,
		showWarranty: true
	},

	'V': {
		specificFields: [
			{ id: 'screen_size', label: 'Screen Size', type: 'text', required: true },
			{ id: 'resolution', label: 'Resolution', type: 'text' },
			{ id: 'panel_type', label: 'Panel Type', type: 'text' },
			{ id: 'Software', label: 'Software', type: 'text' },
			{ id: 'SoftwareVersion', label: 'Software Version', type: 'text' },
			{ id: 'SoftwareKey', label: 'Software Key', type: 'text' },
			{ id: 'SoftwareLicence', label: 'Software Licence', type: 'text' },
		],
		showNetwork: false,
		showWarranty: true
	},

	'RP': {
		specificFields: [
			{ id: 'printer_type', label: 'Printer Type', type: 'select', options: ['Inkjet', 'Laser', 'Dot Matrix'] },
			{ id: 'rental_company', label: 'Rental Company', type: 'text' },
			{ id: 'rental_start_date', label: 'Rental Start Date', type: 'date' },
			{ id: 'rental_end_date', label: 'Rental End Date', type: 'date' }
		],
		showNetwork: false,
		showWarranty: false
	},

	'P': {
		specificFields: [
			{ id: 'printer_type', label: 'Printer Type', type: 'select', options: ['Inkjet', 'Laser', 'Dot Matrix'] },
			{ id: 'print_speed', label: 'Print Speed', type: 'text' },
			{ id: 'paper_capacity', label: 'Paper Capacity', type: 'text' }
		],
		showNetwork: false,
		showWarranty: true
	},

	'W': {
		specificFields: [
			{ id: 'port_count', label: 'Port Count', type: 'number' },
			{ id: 'switch_type', label: 'Switch Type', type: 'select', options: ['Managed', 'Unmanaged'] },
			{ id: 'speed', label: 'Speed', type: 'text' }
		],
		showNetwork: false,
		showWarranty: true
	},

	'F': {
		specificFields: [
			{ id: 'firewall_type', label: 'Firewall Type', type: 'text' },
			{ id: 'throughput', label: 'Throughput', type: 'text' },
			{ id: 'port_count', label: 'Port Count', type: 'number' }
		],
		showNetwork: true,
		showWarranty: true
	},

	'B': {
		specificFields: [
			{ id: 'recognition_type', label: 'Recognition Type', type: 'select', options: ['Face', 'Fingerprint', 'Both'] },
			{ id: 'capacity', label: 'User Capacity', type: 'number' },
			{ id: 'connectivity', label: 'Connectivity', type: 'select', options: ['Ethernet', 'WiFi', 'Both'] }
		],
		showNetwork: true,
		showWarranty: true
	},

	'Chair': {
		specificFields: [
			{ id: 'chair_type', label: 'Chair Type', type: 'text' },
			{ id: 'material', label: 'Material', type: 'text' },
			{ id: 'color', label: 'Color', type: 'text' }
		],
		showNetwork: false,
		showWarranty: true
	},

	'RAM': {
		specificFields: [
			{ id: 'ram_capacity', label: 'RAM Capacity', type: 'text' },
			{ id: 'ram_type', label: 'RAM Type', type: 'select', options: ['DDR3', 'DDR4', 'DDR5'] },
			{ id: 'speed', label: 'Speed (MHz)', type: 'text' }
		],
		showNetwork: false,
		showWarranty: true
	},

	'HDD': {
		specificFields: [
			{ id: 'storage_capacity', label: 'Storage Capacity', type: 'text' },
			{ id: 'drive_type', label: 'Drive Type', type: 'select', options: ['HDD', 'SSD', 'Hybrid'] },
			{ id: 'HddType', label: 'HDD Type', type: 'select', options: ['SATA', 'NVMe', 'IDE'] }
		],
		showNetwork: false,
		showWarranty: true
	},

	'AC': {
		specificFields: [
			{ id: 'cooling_capacity', label: 'Cooling Capacity (Ton)', type: 'text' },
			{ id: 'energy_rating', label: 'Energy Rating', type: 'text' },
			{ id: 'installation_location', label: 'Installation Location', type: 'text' },
			{ id: 'NoiseLevels', label: 'Noise Levels', type: 'text' }
		],
		showNetwork: false,
		showWarranty: true
	},

	'Vehicle': {
		specificFields: [
			{ id: 'vehicle_type', label: 'Vehicle Type', type: 'select', options: ['Car', 'Bike', 'Truck', 'Van'] },
			{ id: 'registration_no', label: 'Registration No', type: 'text' },
			{ id: 'engine_no', label: 'Engine No', type: 'text' },
			{ id: 'fuel_type', label: 'Fuel Type', type: 'select', options: ['Petrol', 'Diesel', 'Electric', 'Hybrid'] }
		],
		showNetwork: false,
		showWarranty: true
	},
};