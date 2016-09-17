package com.scrumlogic.pushmenow;

import android.os.AsyncTask;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextUtils;
import android.text.TextWatcher;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import com.google.android.gms.gcm.*;
import com.microsoft.windowsazure.messaging.*;
import com.microsoft.windowsazure.notifications.NotificationsManager;

import org.json.JSONObject;

import java.util.UUID;

public class MainActivity extends AppCompatActivity {
    private final static String TAG = MainActivity.class.getSimpleName();

    private final String SENDER_ID = "<YOUR-SENDER-ID>";
    private final String HubName = "<HUB-NAME>";
    private final String HubListenConnectionString = "<HUB-LISTEN-CONNECTION_STRING>";

    private EditText _nameEditText;
    private Button _button;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        _nameEditText = (EditText) findViewById(R.id.editText_name);
        _nameEditText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
                _button.setEnabled(!TextUtils.isEmpty(s));
            }

            @Override
            public void afterTextChanged(Editable s) {

            }
        });

        _button = (Button) findViewById(R.id.button_register);
        _button.setEnabled(!TextUtils.isEmpty(_nameEditText.getText()));
        _button.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                registerPushNotification();
            }
        });

        NotificationsManager.handleNotifications(this, SENDER_ID, MyHandler.class);
    }

    private void registerPushNotification() {
        final UserInfo userInfo = new UserInfo();
        userInfo.setName(_nameEditText.getText().toString());


        new AsyncTask() {
            @Override
            protected void onPreExecute() {
                _button.setEnabled(false);
            }

            @Override
            protected void onPostExecute(Object o) {
                _button.setEnabled(true);
            }

            @Override
            protected Object doInBackground(Object... params) {
                try {
                    GoogleCloudMessaging gcm = GoogleCloudMessaging.getInstance(MainActivity.this);
                    String regid = gcm.register(SENDER_ID);
                    NotificationHub hub = new NotificationHub(HubName, HubListenConnectionString, MainActivity.this);;
                    String registrationId = hub.register(regid, userInfo.getAsTag()).getRegistrationId();

                    ToastNotify("Registered Successfully - RegId : " + registrationId);
                } catch (Exception e) {
                    ToastNotify("Registration Exception Message - " + e.getMessage());
                    return e;
                }
                return null;
            }
        }.execute(null, null, null);
    }

    public void ToastNotify(final String notificationMessage)
    {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Toast.makeText(MainActivity.this, notificationMessage, Toast.LENGTH_LONG).show();
            }
        });
    }
}
