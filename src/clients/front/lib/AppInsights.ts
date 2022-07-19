import {ApplicationInsights} from '@microsoft/applicationinsights-web';
import {ReactPlugin} from '@microsoft/applicationinsights-react-js';

export const reactPlugin = new ReactPlugin();
export const appInsights = new ApplicationInsights({
    config: {
        instrumentationKey: '158da90e-21a9-406c-918b-79ccad7d5364',
        extensions: [reactPlugin],
        enableAutoRouteTracking: true,
        connectionString: 'InstrumentationKey=158da90e-21a9-406c-918b-79ccad7d5364;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/'
    }
});

appInsights.loadAppInsights();