package com.justyy.pptcontroller;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetSocketAddress;
import java.net.SocketAddress;
import java.net.SocketException;
import java.nio.charset.Charset;

import android.content.SharedPreferences;
import android.os.Bundle;
import android.os.StrictMode;
import android.support.v7.app.ActionBarActivity;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;

public class MainActivity extends ActionBarActivity {

	DatagramSocket localSocket;
	SharedPreferences sharedPreferences;
	SharedPreferences.Editor editor;
	EditText et_ip = null;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder()
				.permitAll().build();

		StrictMode.setThreadPolicy(policy);

		try {
			localSocket = new DatagramSocket();
		} catch (SocketException e) {
			// TODO 自动生成的 catch 块
			e.printStackTrace();
		}

		sharedPreferences = getSharedPreferences("PPT_UDP", MODE_PRIVATE);
		editor = sharedPreferences.edit();
		String savedIP = sharedPreferences.getString("IP", "127.0.0.1");

		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);

		et_ip = (EditText) findViewById(R.id.et_ip);
		et_ip.setText(savedIP);

		Button btn_up = (Button) findViewById(R.id.btn_up);
		Button btn_down = (Button) findViewById(R.id.btn_down);
		Button btn_play = (Button) findViewById(R.id.btn_play);
		Button btn_stop = (Button) findViewById(R.id.btn_stop);

		btn_up.setOnClickListener(new OnClickListener() {

			@Override
			public void onClick(View v) {
				String ip = et_ip.getText().toString();
				if (ip.isEmpty())
					return;
				SendKey(ip, -1);
			}
		});

		btn_down.setOnClickListener(new OnClickListener() {

			@Override
			public void onClick(View v) {
				String ip = et_ip.getText().toString();
				if (ip.isEmpty())
					return;
				SendKey(ip, 1);
			}
		});

		btn_play.setOnClickListener(new OnClickListener() {

			@Override
			public void onClick(View v) {
				String ip = et_ip.getText().toString();
				if (ip.isEmpty())
					return;
				SendKey(ip, 0);
			}
		});

		btn_stop.setOnClickListener(new OnClickListener() {

			@Override
			public void onClick(View v) {
				String ip = et_ip.getText().toString();
				if (ip.isEmpty())
					return;
				SendKey(ip, -2);
			}
		});
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		// Handle action bar item clicks here. The action bar will
		// automatically handle clicks on the Home/Up button, so long
		// as you specify a parent activity in AndroidManifest.xml.
		int id = item.getItemId();
		if (id == R.id.action_settings) {
			return true;
		}
		return super.onOptionsItemSelected(item);
	}

	private void SendKey(String serverIP, int flag) {
		String sendStr = "PGDN";
		switch (flag) {
		case 1:
			sendStr = "PGDN";
			break;
		case -1:
			sendStr = "PGUP";
			break;
		case 0:
			sendStr = "START";
			break;
		case -2:
			sendStr = "ESC";
			break;
		default:
			sendStr = "PGDN";
			break;
		}
		byte[] data = sendStr.getBytes(Charset.forName("utf-8"));
		SocketAddress server = new InetSocketAddress(serverIP, 8788);
		DatagramPacket dp = null;

		try {
			dp = new DatagramPacket(data, data.length, server);
			localSocket.connect(new InetSocketAddress(serverIP, 8788));
			localSocket.send(dp);
		} catch (SocketException e) {
			// TODO 自动生成的 catch 块
			e.printStackTrace();
		} catch (IOException e) {
			// TODO 自动生成的 catch 块
			e.printStackTrace();
		}
	}

	@Override
	public void onBackPressed() {
		if (et_ip != null) {
			try {
				new InetSocketAddress(et_ip.getText().toString(), 1111);
				editor.putString("IP", et_ip.getText().toString());
				editor.commit();
			} catch (Exception e) {
				// TODO: handle exception
			}
		}
		super.onBackPressed();
	}
}
