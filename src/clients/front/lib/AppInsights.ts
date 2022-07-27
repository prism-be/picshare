import {ApplicationInsights, ITelemetryItem} from '@microsoft/applicationinsights-web';
import {ReactPlugin} from '@microsoft/applicationinsights-react-js';

const telemetryInitializer = (envelope: ITelemetryItem) => {
    if (envelope.tags) {
        envelope.tags['ai.cloud.role'] = "front";
    }
};

export const reactPlugin = new ReactPlugin();
export const appInsights = new ApplicationInsights({
    config: {
        extensions: [reactPlugin],
        enableAutoRouteTracking: true,
        appId: 'client-front'
    }
});

let appInsightsLoaded = false;

export const loadAppInsights = () => {
    
    if (appInsightsLoaded)
    {
        return;
    }

    appInsightsLoaded = true;
    
    fetch('/api/config/insights')
        .then((response) => {
            response.json().then(data => {
                appInsights.config.instrumentationKey = data.instrumentationKey;
                appInsights.config.connectionString = data.connectionString;
                appInsights.loadAppInsights();
                appInsights.addTelemetryInitializer(telemetryInitializer);
            })
        });
}